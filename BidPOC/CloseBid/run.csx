#r "Microsoft.WindowsAzure.Storage"
#load "..\Entities\BidOffer.csx"
#load "..\Entities\Bids.csx"
#load "..\Entities\BidLogs.csx"
using System.Net;
using Microsoft.WindowsAzure.Storage.Table;

public static void Run(TimerInfo myTimer, IQueryable<BidOffer> inBidOffers, 
    IQueryable<Bids> inBids, CloudTable outBidOffers,
    ICollector<BidLogs> outBidLogs, ICollector<BidLogs> outBidsQueue, 
    TraceWriter log)
{
    var bidExpiredList = inBidOffers.Where(bidOffers => (System.DateTime.Compare(bidOffers.EndDate, System.DateTime.Now) < 0)&&(bidOffers.State.CompareTo("Open")==0)).ToList();
    foreach (var bidexpired in bidExpiredList)
    {
        TableOperation deleteOperation = TableOperation.Delete(bidexpired);
        TableResult deleteResult = outBidOffers.Execute(deleteOperation);
        bidexpired.State = "Closed";
        bidexpired.PartitionKey = bidexpired.State;
        TableOperation insertOperation = TableOperation.InsertOrReplace(bidexpired);
        TableResult insertResult = outBidOffers.Execute(insertOperation);

        var placedBids = inBids.Where(bid => bid.BidOfferID.CompareTo(bidexpired.RowKey) == 0).ToList();
        Bids selectedBid = null;
        foreach(var placedBid in placedBids)
        {
            if (selectedBid == null)
                selectedBid = placedBid;
            else if (System.Convert.ToDouble(selectedBid.BidPrice)< System.Convert.ToDouble(placedBid.BidPrice))
            {
                selectedBid = placedBid;
            }

        }
        if (selectedBid != null)
        {
            BidLogs bidLogs = new BidLogs();
            bidLogs.ID = Guid.NewGuid().ToString();
            bidLogs.PartitionKey = "BidLogs";
            bidLogs.RowKey = bidLogs.ID;
            bidLogs.BidID = selectedBid.ID;
            bidLogs.BidWinnerEmails = selectedBid.BidEmails;
            bidLogs.BidFinalizedDate = System.DateTime.Now;
            bidLogs.Gain = string.Format("{0:N2}", System.Convert.ToDouble(selectedBid.BidPrice) - System.Convert.ToDouble(bidexpired.BasePrice));
            outBidLogs.Add(bidLogs);
            outBidsQueue.Add(bidLogs);
        }
    }
    //log.Info($"C# Timer trigger function executed at: {DateTime.Now}");    
}