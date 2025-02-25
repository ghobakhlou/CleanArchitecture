namespace Application.Common.Interfaces;

public interface IAuditLogService
{
    void Log(string eventType, object extraFields);
}
