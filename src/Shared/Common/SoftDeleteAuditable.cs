namespace Shared.Common;

public abstract class SoftDeleteAuditable<T> : AuditableEntity<T>, ISoftDeleteable
{
}

