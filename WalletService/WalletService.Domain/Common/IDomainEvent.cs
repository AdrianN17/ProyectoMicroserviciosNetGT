using MediatR;

namespace WalletService.Domain.Common
{
    public interface IDomainEvent : INotification
    {
        DateTime OccurredOn { get;  }
    }
}
