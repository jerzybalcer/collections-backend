using Collections.Application.Commands;
using Collections.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Collections.WebApi.Controllers;

[Route("api/")]
[ApiController]
public class ItemController : ControllerBase
{
    private readonly IMediator _mediator;

    public ItemController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("items")]
    public async Task<IActionResult> GetItems()
    {
        var result = await _mediator.Send(new GetItemsQuery());

        return Ok(result);
    }

    [HttpGet("items/favourite")]
    public async Task<IActionResult> GetFavouriteItems()
    {
        var result = await _mediator.Send(new GetFavouriteItemsQuery());

        return Ok(result);
    }

    [HttpPost("item")]
    public async Task<IActionResult> CreateItem([FromBody] NewItem newItem)
    {
        var result = await _mediator.Send(new CreateItemCommand { NewItemData = newItem });

        return Ok(result);
    }

    [HttpGet("item/{id}")]
    public async Task<IActionResult> GetItemDetails(Guid id)
    {
        var result = await _mediator.Send(new GetItemDetailsQuery { ItemId = id });

        return Ok(result);
    }

    [HttpDelete("item/{id}")]
    public async Task<IActionResult> DeleteItem(Guid id)
    {
        var result = await _mediator.Send(new DeleteItemCommand { ItemId = id });

        return Ok(result);
    }

    [HttpPut("item/{id}/favourite")]
    public async Task<IActionResult> ToggleIsFavourite(Guid id)
    {
        var result = await _mediator.Send(new ToggleIsFavouriteCommand { ItemId = id });

        return Ok(result);
    }

    [HttpPut("item/{id}")]
    public async Task<IActionResult> EditItem(Guid id, [FromBody] EditedItem editedItem)
    {
        var result = await _mediator.Send(new EditItemCommand { ItemId = id, EditedItem = editedItem });

        return Ok(result);
    }
}
