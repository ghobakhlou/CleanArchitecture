using System;

namespace Domain.Common;

public abstract class AuditableEntity<T> : AggregateEntity<T>, IAuditableEntity
{
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
}
public interface IAuditableEntity
{
    DateTimeOffset CreatedDate { get; set; }
    DateTimeOffset UpdatedDate { get; set; }
    string CreatedBy { get; set; }
    string UpdatedBy { get; set; }
}
