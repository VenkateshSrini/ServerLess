#r "Microsoft.WindowsAzure.Storage"
#load "..\Entities\Product.csx"
using System.Net;
using Microsoft.WindowsAzure.Storage.Table;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, ICollector<Product> outTable, TraceWriter log)
{
    Product data = await req.Content.ReadAsAsync<Product>();
    //string name = data?.name;

    if (data == null)
    {
        return req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a valid product ID in the body the request body");
    }
    data.ID = Guid.NewGuid().ToString();
    outTable.Add(new Product()
    {
        PartitionKey = data.Category,
        RowKey = data.ID,
        Name = data.Name,
        Description = data.Description,
        Price = data.Price,
        Category = data.Category
    });
    return req.CreateResponse(HttpStatusCode.Created);
}

