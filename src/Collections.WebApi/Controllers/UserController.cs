using Collections.Application.Commands;
using Collections.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Collections.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}/items")]
        public async Task<IActionResult> GetUserItems()
        {
            var result = await _mediator.Send(new GetUserItemsQuery { UserId = Guid.NewGuid() });

            return Ok(result);
        }

        [HttpPost("{id}/item")]
        public async Task<IActionResult> CreateItem()
        {
            var result = await _mediator.Send(new CreateItemCommand { UserId = Guid.NewGuid() });

            return Ok(result);
        }
    }
}
