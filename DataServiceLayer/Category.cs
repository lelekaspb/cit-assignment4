namespace DataServiceLayer;
public class Category
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }

    // Mapping relationships
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
