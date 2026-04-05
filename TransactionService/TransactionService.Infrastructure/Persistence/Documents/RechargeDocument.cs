using Newtonsoft.Json;
using TransactionService.Domain.Entities;
using TransactionService.Domain.Enums;
using TransactionService.Domain.ValueObjects;

namespace TransactionService.Infrastructure.Persistence.Documents
{
    /// <summary>
    /// Documento plano que se serializa/deserializa en Cosmos DB para la entidad Recharge.
    /// </summary>
    public class RechargeDocument
    {
        [JsonProperty("id")]
        public string Id { get; set; } = default!;

        /// <summary>Partition key del container.</summary>
        [JsonProperty("walletId")]
        public string WalletId { get; set; } = default!;

        [JsonProperty("amountValue")]
        public decimal AmountValue { get; set; }

        [JsonProperty("amountCurrency")]
        public string AmountCurrency { get; set; } = default!;

        [JsonProperty("methodType")]
        public string MethodType { get; set; } = default!;

        [JsonProperty("rechargeStatus")]
        public string RechargeStatus { get; set; } = default!;

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

        public static RechargeDocument FromDomain(Recharge r) => new()
        {
            Id             = r.Id.ToString(),
            WalletId       = r.WalletId.ToString(),
            AmountValue    = r.Amount.Value,
            AmountCurrency = r.Amount.Currency.ToString(),
            MethodType     = r.MethodType.ToString(),
            RechargeStatus = r.RechargeStatus.ToString(),
            CreatedAt      = r.CreatedAt,
            LastModifiedAt = r.LastModifiedAt,
            IsDeleted      = r.IsDeleted,
            DeletedAt      = r.DeletedAt
        };

        public Recharge ToDomain()
        {
            var currency = Enum.Parse<CurrencyType>(AmountCurrency);
            var method   = Enum.Parse<MethodType>(MethodType);
            var status   = Enum.Parse<RechargeStatus>(RechargeStatus);

            return Recharge.Reconstitute(
                id:             new RechargeId(Guid.Parse(Id)),
                walletId:       new WalletId(Guid.Parse(WalletId)),
                amount:         Amount.Create(AmountValue, currency),
                methodType:     method,
                rechargeStatus: status,
                createdAt:      CreatedAt,
                lastModifiedAt: LastModifiedAt,
                isDeleted:      IsDeleted,
                deletedAt:      DeletedAt);
        }
    }
}

