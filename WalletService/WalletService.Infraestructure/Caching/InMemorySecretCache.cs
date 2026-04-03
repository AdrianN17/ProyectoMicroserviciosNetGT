namespace WalletService.Infrastructure.Caching
{
    public sealed class InMemorySecretCache
    {
        private IReadOnlyDictionary<string, string>? _data;
        public IReadOnlyDictionary<string, string>? Get() => _data;
        public void Set(IReadOnlyDictionary<string, string> data) => _data = data;
    }
}
