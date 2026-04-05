namespace TransactionService.Application.Common.Helpers
{
    public static class EnumParsing
    {
        public static bool TryParseEnum<TEnum>(string? value, out TEnum result)
            where TEnum : struct, Enum
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                result = default;
                return false;
            }

            return Enum.TryParse<TEnum>(value.Trim(), ignoreCase: true, out result);
        }

        public static bool TryToEnum<TEnum>(string? value, out TEnum result)
            where TEnum : struct, Enum
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                result = default;
                return false;
            }

            return Enum.TryParse(value.Trim(), ignoreCase: true, out result);
        }
    }
}
