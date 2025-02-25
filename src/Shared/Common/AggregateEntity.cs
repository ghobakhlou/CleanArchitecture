namespace Shared.Common;

public abstract class AggregateEntity<T> : Entity<T> ,ISoftDeleteable
{
    public bool IsDeleted { get; set; }
}


public abstract class Entity<T> 
{
    public T Id { get; set; }
}

