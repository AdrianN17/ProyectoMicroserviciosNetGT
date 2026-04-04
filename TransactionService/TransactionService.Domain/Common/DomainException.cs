namespace TransactionService.Domain.Common
{
    public abstract class DomainException : Exception
    {
        public string Code { get; }
        public virtual string? Target { get; init; } // ej: "documentNumber"
        protected DomainException(string code, string message) : base(message)  => Code = code;

        protected DomainException(string code, string message, Exception? inner): base(message, inner) => Code = code;
    }
}
