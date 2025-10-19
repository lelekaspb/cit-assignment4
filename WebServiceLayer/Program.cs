
using DataServiceLayer;
using Mapster;



namespace WebServiceLayer;

public class Program
{ 
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
    
        builder.Configuration.AddJsonFile("config.json");

        var connectionString = builder.Configuration.GetSection("ConnectionString").Value;

        // Add services to the container.

        builder.Services.AddSingleton<IDataService>(new DataService(connectionString));

        builder.Services.AddMapster();

        builder.Services.AddControllers();

        var app = builder.Build();

        // Configure the HTTP request pipeline.

        app.MapControllers();

        app.MapGet("/", () => "Northwind app is running.");

        app.Run();
    }
}