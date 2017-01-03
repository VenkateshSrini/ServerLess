public class BidsDTO
{
    public string ID { get; set; }
    public string BidOfferID { get; set; }
    public List<string> BidEmails { get; set; }
    public DateTime BidDate { get; set; }
    public float BidPrice { get; set; }
}