using MediatR;
using Microsoft.AspNetCore.Mvc;
using WalletService.Application.Wallets.Commands.CreateWallet;
using WalletService.Application.Wallets.Dtos;
using WalletService.Application.Wallets.Queries.GetByIdWallet;
using WalletService.Api.Common;

namespace WalletService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public WalletsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateWalletCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            //return CreatedAtAction(nameof(Get), new { customerId = result }, new { result });

            return result.Match(
                walletId => CreatedAtAction(nameof(GetById), new { walletId }, new { walletId }),
                errors => ErrorOrHttp.MapToProblem(this, errors)
            );
        }

        [HttpGet("{walletId:guid}", Name = "Wallet_GetById")]
        public async Task<IActionResult> GetById(Guid walletId)
        {
            var result = await _mediator.Send(new GetByIdWalletQuery(walletId));

            await Task.Delay(6);

            return result.Match(
                wallet => Ok(wallet),
                errors => ErrorOrHttp.MapToProblem(this, errors)
            );
        }
    }
}
