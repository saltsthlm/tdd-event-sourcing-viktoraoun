using System.Text.Json.Serialization;

namespace EventSourcing.Models;

[JsonConverter(typeof(JsonStringEnumConverter<CurrencyType>))]
public enum CurrencyType
{
  [JsonPropertyName("USD")] Usd,
  [JsonPropertyName("SEK")] Sek,
  [JsonPropertyName("GBP")] Gbp,
}
