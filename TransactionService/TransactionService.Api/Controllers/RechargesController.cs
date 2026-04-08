using MediatR;
using Microsoft.AspNetCore.Mvc;
using TransactionService.Application.Recharge.Queries.GetAllByWalletId;
using TransactionService.Application.Recharge.Queries.GetById;
using TransactionService.Application.Transactions.Commands.CreateRecharge;
using TransactionService.Application.Transactions.Commands.DeleteRecharge;

namespace TransactionService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RechargesController(IMediator mediator) : ControllerBase
    {
        [HttpPost(Name = "Recharge_Create")]
        public async Task<IActionResult> Create(CreateRechargeCommand command, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(command, cancellationToken);

            return result.Match(
                rechargeId => CreatedAtAction(nameof(GetById), new { rechargeId }, new { rechargeId }),
                errors => ErrorOrHttp.MapToProblem(this, errors)
            );
        }

        [HttpGet("{rechargeId:guid}", Name = "Recharge_GetById")]
        public async Task<IActionResult> GetById(Guid rechargeId, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetByIdRechargeQuery(rechargeId), cancellationToken);

            return result.Match(
                Ok,
                errors => ErrorOrHttp.MapToProblem(this, errors)
            );
        }

        [HttpDelete("{rechargeId:guid}", Name = "Recharge_Delete")]
        public async Task<IActionResult> DeleteById(Guid rechargeId, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new DeleteRechargeCommand(rechargeId), cancellationToken);

            return result.Match(
                _ => NoContent(),
                errors => ErrorOrHttp.MapToProblem(this, errors)
            );
        }

        [HttpGet("wallet/{walletId:guid}", Name = "Recharge_GetAllByWalletId")]
        public async Task<IActionResult> GetAllByWalletId(Guid walletId, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetAllByWalletIdRechargeQuery(walletId), cancellationToken);

            return result.Match(
                Ok,
                errors => ErrorOrHttp.MapToProblem(this, errors)
            );
        }
    }
}

