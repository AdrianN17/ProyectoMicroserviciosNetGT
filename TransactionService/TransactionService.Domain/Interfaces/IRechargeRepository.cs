using TransactionService.Domain.Entities;
using TransactionService.Domain.ValueObjects;

namespace TransactionService.Domain.Interfaces;

public interface IRechargeRepository
{
    Task CreateAsync(Recharge recharge);
    Task UpdateAsync(Recharge recharge, CancellationToken cancellationToken = default);
    Task<Recharge?> GetByIdAsync(RechargeId id, CancellationToken cancellationToken = default);
}