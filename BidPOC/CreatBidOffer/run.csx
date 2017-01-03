#r "Microsoft.WindowsAzure.Storage"
#load "..\Entities\BidOffer.csx"
#load "..\Entities\BidOfferDTO.csx"
#load "..\Entities\Product.csx"

using System.Net;
using Microsoft.WindowsAzure.Storage.Table;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, IQueryable<Product> inProduct,   ICollector<BidOffer> outTable, TraceWriter log)
{

    BidOfferDTO data = await req.Content.ReadAsAsync<BidOfferDTO>();
    //string name = data?.name;

    if (data == null)
    {
        return req.CreateResponse(HttpStatusCode.BadRequest, "Please pass valid Bid Offer request body");
    }
    else if ((data.Products==null)||(data.Products.Count ==0))
    {
        return req.CreateResponse(HttpStatusCode.BadRequest, "Bid Offer should have at least one product");
    }
    else
    {
        //Contains is not supported
        //var query = from Product in inProduct
        //            where data.Products.Contains(Product.RowKey)
        //            select Product;
        //if (query.ToList().Count == data.Products.Count)
        bool productMatch = false;
        foreach(string prodId in data.Products)
        {
            var validProduct = inProduct.Where(prodRow => (prodRow.RowKey.CompareTo(prodId) == 0)).ToList();
            if (validProduct.Count > 0)
                productMatch = true;
            else
            {
                productMatch = false;
                break;
            }

        }
        if (!productMatch)
            return req.CreateResponse(HttpStatusCode.BadRequest, "Bid Offer should have valid productID");
        else
        {
            data.ID = Guid.NewGuid().ToString();
            data.State = "Open";
            //data.PartitionKey = data.State;
            //data.RowKey = data.ID;
            outTable.Add(new BidOffer()
            {
                PartitionKey = data.State,
                RowKey = data.ID,
                ID = data.ID,
                Description = data.Description,
                State = data.State,
                //complex data type not supported
                Products = string.Join(",", data.Products.ToArray()),
                StartDate = data.StartDate,
                EndDate = data.EndDate,
                //Decimal, double and float not supported
                BasePrice = string.Format("{0:N2}", data.BasePrice)

            });
            return req.CreateResponse(HttpStatusCode.Created);
        }
    }
    
}

