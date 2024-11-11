using System.Text.Json.Serialization;
using EventSourcing.Models;

namespace EventSourcing.Events;

public record AccountCreatedEvent : Event
{
  [JsonPropertyName("customerId")] 
  public required string CustomerId { get; init; }

  [JsonPropertyName("initialBalance")]
  public required decimal InitialBalance { get; init; }

  [JsonPropertyName("maxBalance")]
  public required decimal MaxBalance { get; init; }

  [JsonPropertyName("currency")]
  public required CurrencyType Currency { get; init; }
}
