namespace WalletService.Application.Common.Exceptions
{
    public abstract class ApplicationException : Exception
    {
        protected ApplicationException(string code, string message) : base(message) => Code = code;
        public string Code { get; }
    }
}
