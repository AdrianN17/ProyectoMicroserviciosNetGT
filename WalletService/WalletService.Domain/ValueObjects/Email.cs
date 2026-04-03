using System.Text.RegularExpressions;

namespace WalletService.Domain.Common
{
    public sealed class Email : ValueObject
    {
        private static readonly Regex _emailRegex = new(@"^[^\s@]+@[^\s@]+\.[^\s@]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public string Address { get; private init; } = default!;

        private Email() { }

        public static Email Create(string address)
        {
            if (string.IsNullOrWhiteSpace(address)) throw new ArgumentException("Email es requerido.", nameof(address));
            var trimmed = address.Trim();
            if (!_emailRegex.IsMatch(trimmed)) throw new ArgumentException("El email es invalido.", nameof(address));

            return new Email() { Address = trimmed };
        }

        public override string ToString() => Address;

        protected override IEnumerable<object?> GetAtomicValues()
        {
            yield return Address;
        }
    }
}
