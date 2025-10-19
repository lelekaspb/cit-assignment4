namespace DataServiceLayer;
public class OrderSummaryModel
{
    public int Id { get; set; }
    public DateTime? OrderDate { get; set; }
    public string? ShipName { get; set; }
    public string? ShipCity { get; set; }
}