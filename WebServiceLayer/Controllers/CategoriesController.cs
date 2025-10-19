using DataServiceLayer;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using WebServiceLayer.Models;

namespace WebServiceLayer.Controllers;

[Route("api/categories")]
[ApiController]
public class CategoriesController : ControllerBase
{

    IDataService _dataService;
    private readonly LinkGenerator _generator;
    private readonly IMapper _mapper;

    public CategoriesController(
        IDataService dataService, 
        LinkGenerator generator,
        IMapper mapper)
    {
        _dataService = dataService;
        _generator = generator;
        _mapper = mapper;
    }

    [HttpGet]
    public IActionResult GetCategories()
    {
        var categories = _dataService.GetCategories()
            .Select(x => CreateCategoryModel(x));

        return Ok(categories);
    }

    [HttpGet("{id}", Name = nameof(GetCategory))]
    public IActionResult GetCategory(int id)
    {
        var category = _dataService.GetCategory(id);

        if (category == null)
        {
            return NotFound();
        }

        var model = CreateCategoryModel(category);

        return Ok(model);
    }

    

    [HttpPost]
    public IActionResult CreateCategory(CreateCategoryModel model)
    {
        var category = model.Adapt<Category>();

        _dataService.CreateCategory(category.Name, category.Description);

        return Created();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteCategory(int id)
    {
        if (_dataService.DeleteCategory(id))
        {
            return NoContent();
        }

        return NotFound();
    }



    private CategoryModel CreateCategoryModel(Category category)
    {
        var model = _mapper.Map<CategoryModel>(category);
        model.Url = GetUrl(nameof(GetCategory), new { id = category.Id });
        return model;
    }

    private string? GetUrl(string endpointName, object values)
    {
        return _generator.GetUriByName(HttpContext, endpointName, values);
    }
}