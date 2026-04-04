namespace TransactionService.Domain.Common
{
    public abstract class AuditableEntity<TId> : Entity<TId>
    {
        protected AuditableEntity() { }

        protected AuditableEntity(TId id) : base(id) { }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? LastModifiedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        protected void SetCreated()
        {
            CreatedAt = DateTime.Now;
        }

        protected void SetCreated(DateTime now)
        {
            CreatedAt = now;
        }

        protected void SetModified()
        {
            LastModifiedAt = DateTime.Now;
        }
        protected void SetModified(DateTime now)
        {
            LastModifiedAt = now;
        }

        protected void SetDeleted()
        {
            IsDeleted = true;
            DeletedAt = DateTime.Now;
        }
    }
}
