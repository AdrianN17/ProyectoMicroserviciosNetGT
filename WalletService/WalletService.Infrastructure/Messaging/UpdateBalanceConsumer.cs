using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using WalletService.Application.Wallets.Commands.UpdateBalance;

namespace WalletService.Infrastructure.Messaging;

public sealed class UpdateBalanceConsumer : IConsumer<SendOperation>
{
    private readonly ISender _sender;
    private readonly ILogger<UpdateBalanceConsumer> _logger;

    public UpdateBalanceConsumer(ISender sender, ILogger<UpdateBalanceConsumer> logger)
    {
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Consume(ConsumeContext<SendOperation> context)
    {
        var message = context.Message;

        _logger.LogInformation("Message received from Service Bus for WalletId {WalletId}", message.WalletId);

        if (!Guid.TryParse(message.WalletId, out var walletId))
        {
            _logger.LogError("Invalid WalletId format received: {WalletId}", message.WalletId);
            return;
        }

        if (!decimal.TryParse(message.Amount, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var amount))
        {
            _logger.LogError("Invalid Amount format received: {Amount}", message.Amount);
            return;
        }

        var command = new UpdateBalanceCommand(
            Type: message.Type,
            WalletId: walletId,
            Amount: amount,
            Currency: message.Currency
        );

        var result = await _sender.Send(command, context.CancellationToken);

        if (result.IsError)
            _logger.LogError("UpdateBalanceCommand failed for WalletId {WalletId}: {Errors}",
                walletId, string.Join(", ", result.Errors.Select(e => e.Description)));
        else
            _logger.LogInformation("Balance updated successfully for WalletId {WalletId}", result.Value);
    }
}
