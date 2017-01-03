public class BidOfferDTO
{
    public string ID { get; set; }
    public string State { get; set; }
    public string Description { get; set; }
    public List<string> Products { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public double BasePrice { get; set; }
}