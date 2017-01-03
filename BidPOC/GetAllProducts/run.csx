#r "Microsoft.WindowsAzure.Storage"
#load "..\Entities\Product.csx"

using System.Net;
using Microsoft.WindowsAzure.Storage.Table;

public static HttpResponseMessage Run(HttpRequestMessage req, IQueryable<Product> inTable, TraceWriter log)
{
    //var query = from Product in inTable select Product;
    //foreach (Person person in query)
    //{
    //    log.Info($"Name:{person.Name}");
    //}
    return req.CreateResponse(HttpStatusCode.OK, inTable.ToList());
}


