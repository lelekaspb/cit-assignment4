namespace DataServiceLayer;
public class Order
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public DateTime Required { get; set; }
    public DateTime? Shipped { get; set; }
    public double Freight { get; set; }
    public string? ShipName { get; set; }
    public string? ShipCity { get; set; }

    // Mapping relationships
    public ICollection<OrderDetail>? OrderDetails { get; set; }
}
