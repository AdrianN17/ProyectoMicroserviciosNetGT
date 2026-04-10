using MediatR;
using Microsoft.AspNetCore.Mvc;
using WalletService.Api.Mapper;
using WalletService.Api.Schema;
using WalletService.Application.Wallets.Queries.GetByIdWallet;

namespace WalletService.Api.Controllers;

[Route("api/wallets")]
[ApiController]
public class WalletsController(IMediator mediator) : ControllerBase
{
    // POST /wallets
    [HttpPost(Name = "Wallet_Create")]
    [ProducesResponseType(typeof(WalletSchemaIdResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create(
        [FromBody] WalletSchemaRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request.ToCreateWalletCommand(), cancellationToken);

        return result.Match(
            walletId => Ok(walletId.ToSchemaIdResponse()),
            errors   => ErrorOrHttp.MapToProblem(this, errors)
        );
    }

    // GET /wallets/{walletId}
    [HttpGet("{walletId:guid}", Name = "Wallet_GetById")]
    [ProducesResponseType(typeof(WalletSchemaResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid walletId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetByIdWalletQuery(walletId), cancellationToken);

        return result.Match(
            dto    => Ok(dto.ToSchemaResponse()),
            errors => ErrorOrHttp.MapToProblem(this, errors)
        );
    }

    // PATCH /wallets/{walletId}
    [HttpPatch("{walletId:guid}", Name = "Wallet_Update")]
    [ProducesResponseType(typeof(WalletSchemaIdResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid walletId,
        [FromBody] WalletPatchSchemaRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request.ToUpdateWalletCommand(walletId), cancellationToken);

        return result.Match(
            id     => Ok(id.ToSchemaIdResponse()),
            errors => ErrorOrHttp.MapToProblem(this, errors)
        );
    }

    // DELETE /wallets/{walletId}
    [HttpDelete("{walletId:guid}", Name = "Wallet_Delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid walletId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(walletId.ToDeleteWalletCommand(), cancellationToken);

        return result.Match(
            _ =>      NoContent(),
            errors => ErrorOrHttp.MapToProblem(this, errors)
        );
    }
}

