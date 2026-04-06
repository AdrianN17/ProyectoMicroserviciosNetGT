namespace TransactionService.Application.Transactions.Dtos;

public record ExchangeDto(
    string currency,
    decimal value
    );