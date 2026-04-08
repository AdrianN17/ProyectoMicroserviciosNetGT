﻿namespace WalletService.Application.Commmon.Interfaces;

/// <summary>
/// Contrato para el consumer de eventos de creación de wallet.
/// Definido en Application para mantener la independencia de infraestructura.
/// </summary>
public interface IConsumer
{
    Task ConsumeAsync(string rawMessage, CancellationToken cancellationToken = default);
}

