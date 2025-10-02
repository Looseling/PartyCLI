namespace PartyCli.Domain.Models;

public record AuditLogEntry(string Action, DateTime Timestamp)
{
    public AuditLogEntry(string action) : this(action, DateTime.UtcNow)
    {
    }
}