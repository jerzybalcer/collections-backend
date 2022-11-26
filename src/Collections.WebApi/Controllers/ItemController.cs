﻿using Collections.Application.Commands;
using Collections.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Collections.WebApi.Controllers;

[Route("api/item")]
[ApiController]
public class ItemController : ControllerBase
{
    private readonly IMediator _mediator;

    public ItemController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetItemDetails(Guid id)
    {
        var result = await _mediator.Send(new GetItemDetailsQuery { ItemId = id });

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteItem(Guid id)
    {
        var result = await _mediator.Send(new DeleteItemCommand { ItemId = id });

        return Ok(result);
    }
}