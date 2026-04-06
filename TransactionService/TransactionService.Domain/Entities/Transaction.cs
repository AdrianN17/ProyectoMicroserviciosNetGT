﻿using TransactionService.Domain.Common;
using TransactionService.Domain.Enums;
using TransactionService.Domain.Exceptions;
using TransactionService.Domain.ValueObjects;

namespace TransactionService.Domain.Entities;

public class Transaction : AggregateRoot<TransactionId>
{
    public WalletId FromWalletId { get; private set; }
    public WalletId ToWalletId { get; private set; }
    public Amount Amount { get; private set; }
    
    public TransactionStatus TransactionStatus { get; private set; }
    
    public SourceType SourceType { get; private set; }

    private Transaction()
    {
    }

    public static Transaction Create(Guid fromWalletId, Guid toWalletId, decimal amount, CurrencyType currency, SourceType sourceType)
    {
        var errors = ValidateFieldsRequired(fromWalletId, toWalletId, amount, currency, sourceType);

        if (errors.Any())
            throw new DomainValidationException("transaction.invalid", "Validation failed", errors);

        var transaction = new Transaction();

        try
        {
            transaction = new Transaction
            {
                Id = TransactionId.NewId(),
                FromWalletId = new WalletId(fromWalletId),
                ToWalletId = new WalletId(toWalletId),
                Amount = Amount.Create(amount, currency),
                TransactionStatus = TransactionStatus.PROCESANDO,
                SourceType = sourceType
            };
        }
        catch (InvalidValueObjectException iv)
        {
            // Mapear errores de ValueObjects a las claves esperadas por Transaction
            var errorsVo = new Dictionary<string, string[]>();

            foreach (var kv in iv.Errors)
            {
                var key = kv.Key ?? string.Empty;

                var targetKey = key switch
                {
                    "fromWalletId"  => "fromWalletId",
                    "toWalletId"  => "toWalletId",
                    "walletId"  => "walletId",
                    "amount" or "value" or "money" => "amount",
                    "currency" => "currency",
                    "sourceType" => "sourceType",
                    _ => key
                };

                errorsVo[targetKey] = kv.Value;
            }

            // Fusionar errores obtenidos desde los VOs con los errores existentes
            foreach (var kv in errorsVo)
            {
                if (errors.ContainsKey(kv.Key))
                {
                    var merged = errors[kv.Key].Concat(kv.Value).Distinct().ToArray();
                    errors[kv.Key] = merged;
                }
                else
                {
                    errors[kv.Key] = kv.Value;
                }
            }
        }

        return errors.Count != 0
            ? throw new DomainValidationException("transaction.invalid", "Validation failed", errors)
            : transaction;
    }

    public static Dictionary<string, string[]> ValidateFieldsRequired(Guid fromWalletId, Guid toWalletId,
        decimal amount, CurrencyType currency, SourceType sourceType)
    {
        var errors = new Dictionary<string, string[]>();

        if (fromWalletId == Guid.Empty)
            errors["fromWalletId"] = ["El identificador de la billetera origen es requerido."];

        if (toWalletId == Guid.Empty)
            errors["toWalletId"] = ["El identificador de la billetera destino es requerido."];
        
        if (fromWalletId == toWalletId)
            errors["walletId"] = ["El identificador de la billetera origen y destino no deben ser iguales."];

        if (amount <= 0m)
            errors["amount"] = ["El monto debe ser mayor a cero."];

        if (!Enum.IsDefined(typeof(CurrencyType), currency) || currency.Equals(default(CurrencyType)))
            errors["currency"] = ["El tipo de moneda es requerido y debe ser válido."];
        
        if (!Enum.IsDefined(typeof(SourceType), sourceType) || currency.Equals(default(SourceType)))
            errors["currency"] = ["El origen es requerido y debe ser válido."];

        return errors;
    }
    
    public void SoftDelete()
    {
        SetDeleted();
    }
}