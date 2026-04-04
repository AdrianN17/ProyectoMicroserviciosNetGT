
namespace TransactionService.Domain.Common
{
    public abstract class ValueObject
    {
        protected abstract IEnumerable<object?> GetAtomicValues();

        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != GetType()) return false;

            var other = (ValueObject)obj;

            return GetAtomicValues().SequenceEqual(other.GetAtomicValues());
        }

        public override int GetHashCode()
        {
            unchecked
            {
                const int seed = 17;
                const int modifier = 31;

                return GetAtomicValues()
                    .Where(x => x != null)
                    .Aggregate(seed, (current, obj) => current * modifier + obj!.GetHashCode());
            }
        }
    }
}
