using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sales.Application.Sales.Commands.CancelSale;
using Sales.Application.Sales.Commands.CreateSale;
using Sales.Application.Sales.Commands.DeleteSale;
using Sales.Application.Sales.Commands.UpdateSale;
using Sales.Application.Sales.Queries.GetSaleById;
using Sales.Application.Sales.Queries.GetSales;

namespace Sales.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _mediator.Send(new GetSalesQuery(page, pageSize));
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetSaleByIdQuery(id));
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSaleCommand command)
        {
            if (command is null) return BadRequest();

            var saleDto = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = saleDto.Id }, null);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSaleCommand command)
        {
            if (command is null) return BadRequest();

            var cmd = command with { Id = id };
            await _mediator.Send(cmd);
            return NoContent();
        }

        [HttpPatch("{id:guid}/cancel")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            await _mediator.Send(new CancelSaleCommand(id));
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteSaleCommand(id));
            return NoContent();
        }
    }
}
