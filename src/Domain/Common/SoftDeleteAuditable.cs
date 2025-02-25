namespace Domain.Common;

public abstract class SoftDeleteAuditable<T> : AuditableEntity<T>, ISoftDeleteable
{
    public bool IsDeleted { get; set; }
}

