using DataServiceLayer;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using WebServiceLayer.Models;

namespace WebServiceLayer.Controllers;

[Route("api/products")]
[ApiController]
public class ProductsController : ControllerBase
{

    IDataService _dataService;
    private readonly LinkGenerator _generator;
    private readonly IMapper _mapper;

    public ProductsController(
        IDataService dataService, 
        LinkGenerator generator,
        IMapper mapper)
    {
        _dataService = dataService;
        _generator = generator;
        _mapper = mapper;
    }

    [HttpGet("{id}", Name = nameof(GetProduct))]
    public IActionResult GetProduct(int id)
    {
        var product = _dataService.GetProduct(id);

        if (product == null)
        {
            return NotFound();
        }

        var model = CreateProductModel(product);

        return Ok(model);
    }

    [HttpGet("category/{categoryId}")]
    public IActionResult GetProductsByCategory(int categoryId)
    {
        var products = _dataService.GetProductsByCategory(categoryId)
            .Select(x => CreateProductModel(x));

        if (products == null || !products.Any())
        {
            return NotFound();
        }

        return Ok(products);
    }

    [HttpGet]
    public IActionResult GetProductsByName(string name)
    {
        var products = _dataService.GetProductsByName(name);

        if (products == null || !products.Any())
        {
            return NotFound();
        }

        return Ok(products);
    }


    private ProductModel CreateProductModel(Product product)
    {
        var model = _mapper.Map<ProductModel>(product);
        model.Url = GetUrl(nameof(GetProduct), new { id = product.Id });
        return model;
    }

    private string? GetUrl(string endpointName, object values)
    {
        return _generator.GetUriByName(HttpContext, endpointName, values);
    }
}
