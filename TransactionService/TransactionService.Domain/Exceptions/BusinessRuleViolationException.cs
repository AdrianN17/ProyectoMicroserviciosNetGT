using TransactionService.Domain.Common;

namespace TransactionService.Domain.Exceptions
{
    public sealed class BusinessRuleViolationException : DomainException
    {
        public BusinessRuleViolationException(string code, string message): base(code, message) { } 
    }
}
