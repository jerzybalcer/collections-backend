using Collections.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Collections.WebApi.Controllers;

[Route("api")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetAllCategories()
    {
        var result = await _mediator.Send(new GetAllCategoriesQuery());

        return Ok(result);
    }

    [HttpGet("category/{id}/tags")]
    public async Task<IActionResult> GetTagsForCategory(Guid id)
    {
        var result = await _mediator.Send(new GetTagsForCategoryQuery { CategoryId = id });

        return Ok(result);
    }
}
