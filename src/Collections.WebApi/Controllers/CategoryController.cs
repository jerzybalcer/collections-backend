using Collections.Application.Commands;
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

    [HttpGet("category/{id}")]
    public async Task<IActionResult> GetCategoryDetails(Guid id)
    {
        var result = await _mediator.Send(new GetCategoryDetailsQuery { CategoryId = id });

        return Ok(result);
    }

    [HttpDelete("category/{id}")]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        var result = await _mediator.Send(new DeleteCategoryCommand { CategoryId = id });

        return Ok(result);
    }

    [HttpPut("category/{id}")]
    public async Task<IActionResult> EditCategory(Guid id, [FromBody] EditedCategory editedCategory)
    {
        var result = await _mediator.Send(new EditCategoryCommand { CategoryId = id, EditedCategory = editedCategory });

        return Ok(result);
    }
}
