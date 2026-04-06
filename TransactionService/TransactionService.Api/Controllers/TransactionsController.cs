using MediatR;
using Microsoft.AspNetCore.Mvc;
using TransactionService.Application.Transactions.Commands.CreateTransaction;
using TransactionService.Application.Transactions.Commands.DeleteTransaction;
using TransactionService.Application.Transactions.Queries;

namespace TransactionService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController(IMediator mediator) : ControllerBase
    {
        [HttpPost(Name = "Transaction_Create")]
        public async Task<IActionResult> Create(CreateTransactionCommand command, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(command, cancellationToken);

            return result.Match(
                transactionId => CreatedAtAction(nameof(GetById), new { transactionId }, new { transactionId }),
                errors => ErrorOrHttp.MapToProblem(this, errors)
            );
        }

        [HttpGet("{transactionId:guid}", Name = "Transaction_GetById")]
        public async Task<IActionResult> GetById(Guid transactionId, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetByIdTransactionQuery(transactionId), cancellationToken);

            return result.Match(
                Ok,
                errors => ErrorOrHttp.MapToProblem(this, errors)
            );
        }

        [HttpDelete("{transactionId:guid}", Name = "Transaction_Delete")]
        public async Task<IActionResult> DeleteById(Guid transactionId, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new DeleteTransactionCommand(transactionId), cancellationToken);

            return result.Match(
                _ => NoContent(),
                errors => ErrorOrHttp.MapToProblem(this, errors)
            );
        }
    }
}

