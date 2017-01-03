#r "Microsoft.WindowsAzure.Storage"
using System;
using Microsoft.WindowsAzure.Storage.Table;

public class BidOffer : TableEntity
{
    public string ID { get; set; }
    public string State { get; set; }
    public string Description { get; set; }
    public string Products { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    public string BasePrice { get; set; }
}