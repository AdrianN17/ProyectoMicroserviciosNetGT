namespace WalletService.Domain.Common
{
    public static class Guard
    {
        public static void AgainstNullOrWhiteSpace(string? value, string message)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new BusinessRuleViolationException("", message);
        }

        public static void Against(bool condition, string message)
        {
            if (condition) throw new BusinessRuleViolationException("", message);
        }
    }
}
