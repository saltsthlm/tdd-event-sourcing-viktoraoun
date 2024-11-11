using System.Text.Json.Serialization;

namespace EventSourcing.Events;

public record DeactivationEvent : Event
{
  [JsonPropertyName("reason")] 
  public required string Reason { get; init; }
}
