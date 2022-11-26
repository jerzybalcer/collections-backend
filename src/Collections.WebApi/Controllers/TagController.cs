using Collections.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Collections.WebApi.Controllers
{
    [Route("api")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TagController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("tags")]
        public async Task<IActionResult> GetAllCategories()
        {
            var result = await _mediator.Send(new GetAllTagsQuery());

            return Ok(result);
        }
    }
}
