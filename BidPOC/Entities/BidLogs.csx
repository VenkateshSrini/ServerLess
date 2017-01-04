#r "Microsoft.WindowsAzure.Storage"
using System;
using Microsoft.WindowsAzure.Storage.Table;

public class BidLogs : TableEntity
{
    public string ID { get; set; }
    public string BidID { get; set; }
    public string BidWinnerEmails { get; set; }
    public DateTime BidFinalizedDate { get; set; }
    public string Gain { get; set; }
}