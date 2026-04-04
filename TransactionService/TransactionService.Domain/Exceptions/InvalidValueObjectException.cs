namespace WalletService.Domain.Exceptions
{
    internal sealed class InvalidValueObjectException : DomainValidationException
    {
        public InvalidValueObjectException(
            string code, 
            string message,
            IReadOnlyDictionary<string, string[]> errors
        ) : base(code, message, errors) { }
    }
}
