using MediatR;
using Microsoft.AspNetCore.Mvc;
using WalletService.Application.Wallets.Commands.CreateWallet;
using WalletService.Application.Wallets.Commands.DeleteWallet;
using WalletService.Application.Wallets.Queries.GetByIdWallet;
using WalletService.Application.Wallets.Commands.UpdateWallet;
using WalletService.Application.Wallets.Queries.GetLimitByIdWalletLimit;

namespace WalletService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletsController(IMediator mediator) : ControllerBase
    {
        [HttpPost(Name = "Wallet_Create")]
        public async Task<IActionResult> Create(CreateWalletCommand command, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(command, cancellationToken);

            return result.Match(
                walletId => CreatedAtAction(nameof(GetById), new { walletId }, new { walletId }),
                errors => ErrorOrHttp.MapToProblem(this, errors)
            );
        }

        [HttpGet("{walletId:guid}", Name = "Wallet_GetById")]
        public async Task<IActionResult> GetById(Guid walletId)
        {
            var result = await mediator.Send(new GetByIdWalletQuery(walletId));

            await Task.Delay(6);

            return result.Match(
                Ok,
                errors => ErrorOrHttp.MapToProblem(this, errors)
            );
        }
        
        [HttpPatch("{walletId:guid}", Name = "Wallet_Update")]
        public async Task<IActionResult> Update(Guid walletId, UpdateWalletCommand command, CancellationToken cancellationToken)
        {
            var commandWithId = command with { WalletId = walletId };
            var result = await mediator.Send(commandWithId, cancellationToken);
            
            return result.Match(
                walletId => CreatedAtAction(nameof(GetById), new { walletId }, new { walletId }),
                errors => ErrorOrHttp.MapToProblem(this, errors)
            );
        }
        
        [HttpDelete("{walletId:guid}", Name = "Wallet_Delete")]
        public async Task<IActionResult> DeleteById(Guid walletId)
        {
            var result = await mediator.Send(new DeleteWalletCommand(walletId));

            await Task.Delay(6);

            return result.Match(
                _ => NoContent(),
                errors => ErrorOrHttp.MapToProblem(this, errors)
            );
        }
        
        [HttpGet("limit/{walletId:guid}", Name = "WalletLimit_GetById")]
        public async Task<IActionResult> GetWalletLmitById(Guid walletId)
        {
            var result = await mediator.Send(new GetByIdWalletLimitQuery(walletId));

            await Task.Delay(6);

            return result.Match(
                Ok,
                errors => ErrorOrHttp.MapToProblem(this, errors)
            );
        }
    }
}
