using Newtonsoft.Json;
using TransactionService.Domain.Entities;
using TransactionService.Domain.Enums;
using TransactionService.Domain.ValueObjects;

namespace TransactionService.Infrastructure.Persistence.Documents
{
    /// <summary>
    /// Documento plano que se serializa/deserializa en Cosmos DB para la entidad Transaction.
    /// El SDK de Cosmos usa Newtonsoft.Json por defecto.
    /// </summary>
    public class TransactionDocument
    {
        [JsonProperty("id")]
        public string Id { get; set; } = default!;

        /// <summary>Partition key del container.</summary>
        [JsonProperty("fromWalletId")]
        public string FromWalletId { get; set; } = default!;

        [JsonProperty("toWalletId")]
        public string ToWalletId { get; set; } = default!;

        [JsonProperty("amountValue")]
        public decimal AmountValue { get; set; }

        [JsonProperty("amountCurrency")]
        public string AmountCurrency { get; set; } = default!;

        [JsonProperty("transactionStatus")]
        public string TransactionStatus { get; set; } = default!;

        [JsonProperty("sourceType")]
        public string SourceType { get; set; } = default!;

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("lastModifiedAt")]
        public DateTime? LastModifiedAt { get; set; }

        [JsonProperty("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonProperty("deletedAt")]
        public DateTime? DeletedAt { get; set; }

        // ──────────────────────────────────────────────────
        // Mappings  Domain → Document  /  Document → Domain
        // ──────────────────────────────────────────────────

        public static TransactionDocument FromDomain(Transaction t) => new()
        {
            Id                = t.Id.ToString(),
            FromWalletId      = t.FromWalletId.ToString(),
            ToWalletId        = t.ToWalletId.ToString(),
            AmountValue       = t.Amount.Value,
            AmountCurrency    = t.Amount.Currency.ToString(),
            TransactionStatus = t.TransactionStatus.ToString(),
            SourceType        = t.SourceType.ToString(),
            CreatedAt         = t.CreatedAt,
            LastModifiedAt    = t.LastModifiedAt,
            IsDeleted         = t.IsDeleted,
            DeletedAt         = t.DeletedAt
        };

        public Transaction ToDomain()
        {
            // Reconstruimos la entidad usando reflexión privada (sin exponer constructores públicos)
            var currency = Enum.Parse<CurrencyType>(AmountCurrency);
            var status   = Enum.Parse<TransactionStatus>(TransactionStatus);
            var source   = Enum.Parse<SourceType>(SourceType);

            var transaction = Transaction.Reconstitute(
                id:                new TransactionId(Guid.Parse(Id)),
                fromWalletId:      new WalletId(Guid.Parse(FromWalletId)),
                toWalletId:        new WalletId(Guid.Parse(ToWalletId)),
                amount:            Amount.Create(AmountValue, currency),
                transactionStatus: status,
                sourceType:        source,
                createdAt:         CreatedAt,
                lastModifiedAt:    LastModifiedAt,
                isDeleted:         IsDeleted,
                deletedAt:         DeletedAt);

            return transaction;
        }
    }
}

