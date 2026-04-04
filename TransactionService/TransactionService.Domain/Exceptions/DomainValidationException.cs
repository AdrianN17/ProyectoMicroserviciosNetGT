namespace WalletService.Domain.Exceptions
{
    public class DomainValidationException : DomainException
    {
        public DomainValidationException(string code, string message, IReadOnlyDictionary<string, string[]> errors) 
            : base(code, message) => Errors = errors;
        public IReadOnlyDictionary<string, string[]> Errors { get; }
    }
}
