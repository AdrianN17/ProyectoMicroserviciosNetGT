using MediatR;
using Microsoft.AspNetCore.Mvc;
using TransactionService.Api.Mapper;
using TransactionService.Application.Recharge.Queries.GetAllByWalletId;
using WalletService.Client;

namespace TransactionService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RechargesController(IMediator mediator) : ControllerBase
    {
        [HttpPost(Name = "Recharge_Create")]
        public async Task<IActionResult> Create([FromBody] RechargeSchemaRequest schema, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(schema.ToCommand(), cancellationToken);

            return result.Match(
                rechargeId => Ok(rechargeId.ToRechargeIdResponse()),
                errors => ErrorOrHttp.MapToProblem(this, errors)
            );
        }

        [HttpDelete("{rechargeId:guid}", Name = "Recharge_Delete")]
        public async Task<IActionResult> DeleteById(Guid rechargeId, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(rechargeId.ToDeleteRechargeCommand(), cancellationToken);

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
                recharges => Ok(recharges.Select(r => r.ToResponse())),
                errors => ErrorOrHttp.MapToProblem(this, errors)
            );
        }
    }
}
