using System.Collections.Generic;

namespace Domain.Common;

public interface ISoftDeleteable
{
    bool IsDeleted { get; set; }
}

public abstract class TenantableBase <T>: AuditableEntity<T>
{
   public List<Tenant> Tenants { get; set; }
}


