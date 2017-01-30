#r "Microsoft.WindowsAzure.Storage"
#load "..\Entities\Product.csx"
using System.Net;
using Microsoft.WindowsAzure.Storage.Table;

public static HttpResponseMessage Run(HttpRequestMessage req, IQueryable<Product> inProduct, 
    string productId,string description,
    CloudTable outTable, TraceWriter log)
{

    if (string.IsNullOrEmpty(productId))
    {
        return new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("Product ID is Mandatory.")
        };
    }
    if (string.IsNullOrWhiteSpace(description))
    {
        return new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("description cannot be empty")
        };
    }
    log.Info($"product ID ={productId} Description={description}");
    var productList = inProduct.Where(paramProduct => paramProduct.RowKey.CompareTo(productId) == 0).ToList();
    if (productList.Count == 0)
    {
        return new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("ID with the given product does not exist")
        };
    }
    Product product = productList[0];
    product.Description = description;
    TableOperation updateOperation = TableOperation.Replace(product);
    TableResult result = outTable.Execute(updateOperation);    
    return new HttpResponseMessage((HttpStatusCode)result.HttpStatusCode);
}

