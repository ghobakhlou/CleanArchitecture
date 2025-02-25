using Audit.Core;
using Application.Common.Interfaces;

namespace Infrastructure.Services;

public class AuditLogService : IAuditLogService
{
    public void Log(string eventType, object extraFields)
    {
        AuditScope.Log(eventType, extraFields);
    }
}
