namespace TransactionService.Application.Exchange.Dtos;

public record ExchangeDto(
    string currency,
    decimal value
    );