#r "Microsoft.WindowsAzure.Storage"
using System;
using Microsoft.WindowsAzure.Storage.Table;

public class Bids: TableEntity
{
    public string ID { get; set; }
    public string BidOfferID { get; set; }
    public string BidEmails { get; set; }
    public DateTime BidDate { get; set; }
    public string BidPrice { get; set; }
}