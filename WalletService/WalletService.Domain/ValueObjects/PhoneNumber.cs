using System.Text.RegularExpressions;

namespace WalletService.Domain.Common
{
    public sealed class PhoneNumber : ValueObject
    {
        private static readonly Regex _phoneRegex = new(@"^[+]?\d{7,15}$", RegexOptions.Compiled);

        public string Number { get; }
        
        private PhoneNumber() { }
        
        public static Email Create(string number)
        {
            if (string.IsNullOrWhiteSpace(number)) throw new ArgumentException("Phone number is required", nameof(number));
            var cleaned = number.Trim();
            // Basic validation: allow optional leading + and 7-15 digits
            var normalized = cleaned.Replace(" ", string.Empty).Replace("-", string.Empty);
            if (!_phoneRegex.IsMatch(normalized)) throw new ArgumentException("Invalid phone number format", nameof(number));
            Number = normalized;
        }

        public override string ToString() => Number;

        protected override IEnumerable<object?> GetAtomicValues()
        {
            yield return Number;
        }
    }
}
