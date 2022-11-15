using Collections.Application.Commands;
using Collections.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Collections.WebApi.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}/items")]
        public async Task<IActionResult> GetUserItems(Guid id)
        {
            var result = await _mediator.Send(new GetUserItemsQuery { UserId = id });

            return Ok(result);
        }

        [HttpPost("{id}/item")]
        public async Task<IActionResult> CreateItem(Guid id)
        {
            var result = await _mediator.Send(new CreateItemCommand { UserId = id });

            return Ok(result);
        }
    }
}
