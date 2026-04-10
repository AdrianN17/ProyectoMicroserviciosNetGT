using MediatR;
using Microsoft.AspNetCore.Mvc;
using TransactionService.Application.Transactions.Commands.CreateTransaction;
using TransactionService.Application.Transactions.Commands.DeleteTransaction;
using TransactionService.Application.Transactions.Queries.GetAllByFromWalletId;

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
                transactionId => CreatedAtAction(nameof(Create), new { transactionId }, new { transactionId }),
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

        [HttpGet("wallet/{fromWalletId:guid}", Name = "Transaction_GetAllByFromWalletId")]
        public async Task<IActionResult> GetAllByFromWalletId(Guid fromWalletId, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetAllByFromWalletIdTransactionQuery(fromWalletId), cancellationToken);

            return result.Match(
                Ok,
                errors => ErrorOrHttp.MapToProblem(this, errors)
            );
        }
    }
}

