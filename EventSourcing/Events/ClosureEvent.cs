using System.Text.Json.Serialization;

namespace EventSourcing.Events;

public record ClosureEvent : Event
{
  [JsonPropertyName("reason")] 
  public required string Reason { get; init; }
}
