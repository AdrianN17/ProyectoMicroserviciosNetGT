using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using WalletService.Application.Wallets.Commands.UpdateBalance;

namespace WalletService.Infrastructure.Messaging;

public sealed class UpdateBalanceConsumer : IConsumer<UpdateBalanceCommand>
{
    private readonly ISender _sender;
    private readonly ILogger<UpdateBalanceConsumer> _logger;

    public UpdateBalanceConsumer(ISender sender, ILogger<UpdateBalanceConsumer> logger)
    {
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Consume(ConsumeContext<UpdateBalanceCommand> context)
    {
        var command = context.Message;

        _logger.LogInformation("Message received from Service Bus for WalletId {WalletId}", command.WalletId);

        var result = await _sender.Send(command, context.CancellationToken);

        if (result.IsError)
            _logger.LogError("UpdateBalanceCommand failed for WalletId {WalletId}: {Errors}",
                command.WalletId, string.Join(", ", result.Errors.Select(e => e.Description)));
        else
            _logger.LogInformation("Balance updated successfully for WalletId {WalletId}", result.Value);
    }
}

