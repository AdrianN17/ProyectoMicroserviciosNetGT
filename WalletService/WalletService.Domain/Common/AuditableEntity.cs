namespace WalletService.Domain.Common
{
    public abstract class AuditableEntity<TId, TUser> : Entity<TId> //, IAuditable<TUser>
    {
        protected AuditableEntity() { }

        protected AuditableEntity(TId id) : base(id) { }

        public TUser? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public TUser? LastModifiedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }

        public bool IsDeleted { get; set; }
        public TUser? DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }

        protected void SetCreated(TUser? user)
        {
            CreatedBy = user;
            CreatedAt = DateTime.Now;
        }

        protected void SetCreated(TUser? user, DateTime now)
        {
            CreatedBy = user;
            CreatedAt = now;
        }

        protected void SetModified(TUser? user)
        {
            LastModifiedBy = user;
            LastModifiedAt = DateTime.Now;
        }
        protected void SetModified(TUser? user, DateTime now)
        {
            LastModifiedBy = user;
            LastModifiedAt = now;
        }

        protected void SetDeleted(TUser? user)
        {
            IsDeleted = true;
            DeletedBy = user;
            DeletedAt = DateTime.Now;
        }
    }
}
