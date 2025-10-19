using Microsoft.EntityFrameworkCore;

namespace DataServiceLayer;

public class DataService : IDataService
{
    private readonly string? _connectString;
    private readonly List<Category> _categories = new List<Category>
    {
        new Category{ Id = 1, Name = "Beverages", Description = "Soft drinks, coffees, teas, beers, and ales"},
        new Category{ Id = 2, Name = "Condiments", Description = "Sweet and savory sauces, relishes, spreads, and seasonings"},
        new Category{ Id = 3, Name = "Confections", Description = "Desserts, candies, and sweet breads"},
        new Category{ Id = 4, Name = "Dairy Products", Description = "Cheeses"},
        new Category{ Id = 5, Name = "Grains/Cereals", Description = "Breads, crackers, pasta, and cereal"},
        new Category{ Id = 6, Name = "Meat/Poultry", Description = "Prepared meats"},
        new Category{ Id = 7, Name = "Produce", Description = "Dried fruit and bean curd"},
        new Category{ Id = 8, Name = "Seafood", Description = "Seaweed and fish"}
    };

    private readonly List<Product> _products = new List<Product>();


    public DataService(string? connectString)
    {
        _connectString = connectString;
    }

    /////////////////////////////////////////////////
    // Category
    /////////////////////////////////////////////////
  
    public int GetCategoriesCount()
    {
        using var db = new NorthwindContext(_connectString);
        return db.Categories.Count();
    }

    // 9. Get category by ID
    public Category? GetCategory(int id)
    {
        using var db = new NorthwindContext(_connectString);
        return db.Categories.FirstOrDefault(x => x.Id == id);
    }

    // 10. Get all categories
    public IList<Category> GetCategories()
    {
        using var db = new NorthwindContext(_connectString);
        return db.Categories.ToList();
    }

    // 11. Add a new category
    public Category CreateCategory(string name, string description)
    {
        using var db = new NorthwindContext(_connectString);
        var maxId = db.Categories.Any() ? db.Categories.Max(x => x.Id) : 0;
        var category = new Category { Id = maxId + 1, Name = name, Description = description };
        db.Categories.Add(category);
        db.SaveChanges();
        return category;
    }

    // 12. Update category
    public bool UpdateCategory(int id, string name, string description)
    {
        using var db = new NorthwindContext(_connectString);
        var category = db.Categories.Find(id);
        if (category == null) return false;
        category.Name = name;
        category.Description = description;
        db.Update(category);
        return db.SaveChanges() > 0;
    }

    // 13. Delete category
    public bool DeleteCategory(int id)
    {
        using var db = new NorthwindContext(_connectString);
        var category = db.Categories.Find(id);
        if (category == null) return false;
        db.Categories.Remove(category);
        return db.SaveChanges() > 0;
    }

    /////////////////////////////////////////////////
    // Product
    /////////////////////////////////////////////////

    // 6. Get a single product by ID
    public Product? GetProduct(int id)
    {
        using var db = new NorthwindContext(_connectString);
        return db.Products.Include(p => p.Category).FirstOrDefault(p => p.Id == id);
    }

    // 7. Get products containing a substring
    public IList<ProductSearchModel> GetProductsByName(string search)
    {
        using var db = new NorthwindContext(_connectString);
        return db.Products
            .Include(p => p.Category)
            .Where(p => (p.Name ?? string.Empty).ToLower().Contains((search ?? string.Empty).ToLower()))
            .Select(p => new ProductSearchModel { ProductName = p.Name ?? string.Empty, CategoryName = p.Category != null ? p.Category.Name : string.Empty })
            .ToList();
    }

    // 8. Get products by category ID
    public IList<Product> GetProductsByCategory(int categoryId)
    {
        using var db = new NorthwindContext(_connectString);
        return db.Products.Include(p => p.Category).Where(p => p.CategoryId == categoryId).ToList();
    }

    /////////////////////////////////////////////////
    // Orders
    /////////////////////////////////////////////////

    // 1. Get a single order by ID
    public Order? GetOrder(int id)
    {
        using var db = new NorthwindContext(_connectString);
        return db.Orders            
            .Include(o => o.OrderDetails!)
                .ThenInclude(od => od.Product!)
                    .ThenInclude(p => p.Category!)
            .FirstOrDefault(o => o.Id == id);
    }

    // 2. Get orders by shipping name
    public IList<OrderSummaryModel> GetOrdersByShipName(string shipName)
    {
        using var db = new NorthwindContext(_connectString);
        return db.Orders
            .Where(o => (o.ShipName ?? string.Empty).ToLower().Contains((shipName ?? string.Empty).ToLower()))
            .Select(o => new OrderSummaryModel { Id = o.Id, OrderDate = o.Date, ShipName = o.ShipName ?? string.Empty, ShipCity = o.ShipCity ?? string.Empty })
            .ToList();
    }

    // 3. List all orders
    public IList<OrderSummaryModel> GetAllOrders()
    {
        using var db = new NorthwindContext(_connectString);
        return db.Orders.Select(o => new OrderSummaryModel { Id = o.Id, OrderDate = o.Date, ShipName = o.ShipName ?? string.Empty, ShipCity = o.ShipCity ?? string.Empty }).ToList();
    }

    /////////////////////////////////////////////////
    // Order Details
    /////////////////////////////////////////////////

    // 4. Get details for a specific order ID
    public IList<OrderDetail> GetOrderDetailsByOrderId(int orderId)
    {
        using var db = new NorthwindContext(_connectString);
        return db.OrderDetails.Include(od => od.Product).Where(od => od.OrderId == orderId).ToList();
    }

    // 5. Get details for a specific product ID
    public IList<OrderDetail> GetOrderDetailsByProductId(int productId)
    {
        using var db = new NorthwindContext(_connectString);
     
        return db.OrderDetails
            .Include(od => od.Product)
            .Include(od => od.Order)
            .Where(od => od.ProductId == productId)
            .OrderByDescending(od => od.Order!.Date)
            .ThenBy(od => od.OrderId)
            .ToList();
    }
}


