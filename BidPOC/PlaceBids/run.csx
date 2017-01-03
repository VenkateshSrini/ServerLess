#r "Microsoft.WindowsAzure.Storage"
#load "..\Entities\BidsDTO.csx"
#load "..\Entities\Bids.csx"
#load "..\Entities\BidOffer.csx"
using System.Net;
using Microsoft.WindowsAzure.Storage.Table;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, IQueryable<BidOffer> inBidOffers, ICollector<Bids> outTable, TraceWriter log)
{
    BidsDTO data= await req.Content.ReadAsAsync<BidsDTO>();
    if (data==null)
        return req.CreateResponse(HttpStatusCode.BadRequest, "Please pass valid Bid request body");
    else if (string.IsNullOrWhiteSpace(data.BidOfferID))
        return req.CreateResponse(HttpStatusCode.BadRequest, "Please pass valid BidOfferID request body");
    else if (data.BidEmails.Count ==0)
        return req.CreateResponse(HttpStatusCode.BadRequest, "Bidder email cannot be empty");
    else
    {
        var bidOffer = inBidOffers.Where(bidOfferRow => bidOfferRow.RowKey.CompareTo(data.BidOfferID) == 0).ToList();
        if (bidOffer.Count == 0)
            return req.CreateResponse(HttpStatusCode.BadRequest, "Provided BidOfferID does not exist");
        else if (bidOffer[0].EndDate < data.BidDate)
            return req.CreateResponse(HttpStatusCode.BadRequest, "BidOffer closed");
        else if (bidOffer[0].State.CompareTo("Open") != 0)
            return req.CreateResponse(HttpStatusCode.BadRequest, "BidOffer closed");
        else 
        {
            if (data.BidPrice > 0)
            {
                var basePrice = System.Convert.ToDouble(bidOffer[0].BasePrice);
                if (data.BidPrice < basePrice)
                    return req.CreateResponse(HttpStatusCode.BadRequest, "Bid Price must be greater than base price");
                data.ID = Guid.NewGuid().ToString();
                outTable.Add(new Bids()
                {
                    PartitionKey = "PlacedBids",
                    RowKey = data.ID,
                    ID = data.ID,
                    BidOfferID = data.BidOfferID,
                    BidEmails = string.Join(";", data.BidEmails),
                    BidDate = data.BidDate,
                    BidPrice = string.Format("{0:N3}", data.BidPrice)
                });
                return req.CreateResponse(HttpStatusCode.Created);
            }
            else
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Bid Price cannot be zero");
            }
        }
        
    }
   
}

