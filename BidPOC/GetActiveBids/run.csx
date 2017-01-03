#r "Microsoft.WindowsAzure.Storage"
#load "..\Entities\BidOffer.csx"
#load "..\Entities\BidOfferDTO.csx"
using System.Net;
using Microsoft.WindowsAzure.Storage.Table;
using AutoMapper;
public static HttpResponseMessage Run(HttpRequestMessage req, IQueryable<BidOffer> inTable, TraceWriter log)
{
    
    var bidOffers = inTable.Where(bidOffer => (bidOffer.PartitionKey.CompareTo("Open") == 0)).ToList();
    if (bidOffers.Count > 0)
    {
        Mapper.Initialize(cfg => cfg.CreateMap<BidOffer, BidOfferDTO>().ConvertUsing(new BidOfferConverter()));
        var results = Mapper.Map<List<BidOffer>, List<BidOfferDTO>>(bidOffers);
        return req.CreateResponse(HttpStatusCode.OK, results);
    }
    else
        return req.CreateResponse(HttpStatusCode.OK, "No Active Bids present");

}

public class BidOfferConverter : ITypeConverter<BidOffer,BidOfferDTO>
{
    public BidOfferDTO Convert(BidOffer source, BidOfferDTO destination, ResolutionContext context)
    {
   
        var bidOfrDTO = new BidOfferDTO();
        bidOfrDTO.ID = source.ID;
        bidOfrDTO.State = source.State;
        bidOfrDTO.Description = source.Description;
        bidOfrDTO.Products = new List<string>(source.Products.Split(','));
        bidOfrDTO.StartDate = source.StartDate;
        bidOfrDTO.EndDate = source.EndDate;
        bidOfrDTO.BasePrice = System.Convert.ToDouble(source.BasePrice);
        return bidOfrDTO;


    }
}

