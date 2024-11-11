namespace EventSourcing.Models;

public record LogMessage(
  string Type,
  string Message,
  DateTime Timestamp
);
