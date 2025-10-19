namespace DataServiceLayer;
public class OrderDetail
{
    public int OrderId { get; set; }
    public int ProductId { get; set; }

    // Tests expect numeric values as doubles for UnitPrice/Quantity/Discount in many asserts
    public double UnitPrice { get; set; }
    public double Quantity { get; set; }
    public double Discount { get; set; }

   // Mapping relationships
    public Order? Order { get; set; }
    public Product? Product { get; set; }
}
