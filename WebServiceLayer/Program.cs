
using DataServiceLayer;
using Mapster;
using Microsoft.EntityFrameworkCore;



namespace WebServiceLayer;

public class Program
{ 
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
    
        builder.Configuration.AddJsonFile("config.json");

        var connectionString = builder.Configuration.GetConnectionString("ConnectionString");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string 'ConnectionString' not found in configuration. Ensure WebServiceLayer/config.json has a 'ConnectionStrings' section with 'ConnectionString'.");
        }

        // Add services to the container.
        // Register the DbContext with the configured PostgreSQL provider so EF has a provider when constructed via DI
        builder.Services.AddDbContext<NorthwindContext>(options => options.UseNpgsql(connectionString));

        // Register DataService to get the connection string from configuration via DI
        builder.Services.AddScoped<IDataService, DataService>(sp =>
        {
            // resolve the DB context options from the scoped provider
            var db = sp.GetRequiredService<NorthwindContext>();
            // DataService still accepts a connection string in its constructor; pass the one from configuration
            return new DataService(connectionString);
        });

        builder.Services.AddMapster();

        builder.Services.AddControllers();

        var app = builder.Build();

        // Configure the HTTP request pipeline.

        app.MapControllers();

        app.MapGet("/", () => "Northwind app is running.");

        app.Run();
    }
}