namespace Domain.Common;

public abstract class AggregateEntity<T>: ISoftDeleteable
{
    public T Id { get; set; }
    public bool IsDeleted { get; set; }
}


