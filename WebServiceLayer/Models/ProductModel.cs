namespace WebServiceLayer.Models;

public class ProductModel
{
    public string? Url { get; set; }
    public string? Name { get; set; }
    public string? CategoryName { get; set; }
    public double UnitPrice { get; set; }
    public string? QuantityPerUnit { get; set; }
    public int UnitsInStock { get; set; }
}