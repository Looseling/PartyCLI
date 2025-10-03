namespace PartyCli.Domain.Models;

public record AuditLogEntry(string Action, DateTime Timestamp);