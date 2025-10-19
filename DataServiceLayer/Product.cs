namespace DataServiceLayer;
public class Product
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public double UnitPrice { get; set; }
    public string? QuantityPerUnit { get; set; }
    public int UnitsInStock { get; set; }

    // Mapping relationships
    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    // Expose CategoryName for tests that expect this flattened property
    public string? CategoryName => Category?.Name;

    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
