using Microsoft.EntityFrameworkCore;


namespace DataServiceLayer;
public class NorthwindContext : DbContext
{
    private readonly string? _connectionString;
    public NorthwindContext(string? connectionString)
    {
        _connectionString = connectionString;
    }

    // Allow constructing with pre-built DbContextOptions (used for InMemory tests)
    public NorthwindContext(DbContextOptions<NorthwindContext> options) : base(options)
    {
        // when options are used we don't set _connectionString
        _connectionString = null;
    }

    // DbSets for each entity
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
        optionsBuilder.EnableSensitiveDataLogging();
        // Only configure Npgsql if there's no pre-configured provider and we have a connection string
        if (!optionsBuilder.IsConfigured && !string.IsNullOrEmpty(_connectionString))
        {
            optionsBuilder.UseNpgsql(_connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
         // Table mappings
        modelBuilder.Entity<Category>().ToTable("categories");
        modelBuilder.Entity<Product>().ToTable("products");
        modelBuilder.Entity<Order>().ToTable("orders");
        modelBuilder.Entity<OrderDetail>().ToTable("order_details");

        // Category
        modelBuilder.Entity<Category>().Property(x => x.Id).HasColumnName("categoryid");
        modelBuilder.Entity<Category>().Property(x => x.Name).HasColumnName("categoryname");
        modelBuilder.Entity<Category>().Property(x => x.Description).HasColumnName("description");

        // Product
        modelBuilder.Entity<Product>().Property(x => x.Id).HasColumnName("productid");
        modelBuilder.Entity<Product>().Property(x => x.Name).HasColumnName("productname");
        modelBuilder.Entity<Product>().Property(x => x.UnitPrice).HasColumnName("unitprice");
        modelBuilder.Entity<Product>().Property(x => x.CategoryId).HasColumnName("categoryid");
    modelBuilder.Entity<Product>().Property(x => x.QuantityPerUnit).HasColumnName("quantityperunit");
    modelBuilder.Entity<Product>().Property(x => x.UnitsInStock).HasColumnName("unitsinstock");

        // OrderDetail - composite key
        modelBuilder.Entity<OrderDetail>()
            .HasKey(od => new { od.OrderId, od.ProductId });

        // Map order detail columns to lowercase column names commonly used in PostgreSQL datasets
        modelBuilder.Entity<OrderDetail>().Property(od => od.OrderId).HasColumnName("orderid");
        modelBuilder.Entity<OrderDetail>().Property(od => od.ProductId).HasColumnName("productid");
        modelBuilder.Entity<OrderDetail>().Property(od => od.UnitPrice).HasColumnName("unitprice");
        modelBuilder.Entity<OrderDetail>().Property(od => od.Quantity).HasColumnName("quantity");
        modelBuilder.Entity<OrderDetail>().Property(od => od.Discount).HasColumnName("discount");

        // Relationships
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId);

        modelBuilder.Entity<OrderDetail>()
            .HasOne(od => od.Order)
            .WithMany(o => o.OrderDetails)
            .HasForeignKey(od => od.OrderId);

        modelBuilder.Entity<OrderDetail>()
            .HasOne(od => od.Product)
            .WithMany(p => p.OrderDetails)
            .HasForeignKey(od => od.ProductId);

        // Order mappings - map properties to lowercase column names
        modelBuilder.Entity<Order>().Property(x => x.Id).HasColumnName("id");
        modelBuilder.Entity<Order>().Property(x => x.Date).HasColumnName("date");
        modelBuilder.Entity<Order>().Property(x => x.Required).HasColumnName("required");
        modelBuilder.Entity<Order>().Property(x => x.Shipped).HasColumnName("shipped");
        modelBuilder.Entity<Order>().Property(x => x.Freight).HasColumnName("freight");
        modelBuilder.Entity<Order>().Property(x => x.ShipName).HasColumnName("shipname");
        modelBuilder.Entity<Order>().Property(x => x.ShipCity).HasColumnName("shipcity");
    }
}