using TransactionService.Domain.Common;

namespace TransactionService.Domain.Exceptions
{
    internal sealed class NotFoundDomainException : DomainException
    {
        public NotFoundDomainException(string code, string message) : base(code, message)
        {
        }
    }
}
