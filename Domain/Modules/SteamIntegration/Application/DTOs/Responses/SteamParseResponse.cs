// DTO для сериализации и десериализации в "data-state.json" с актуальной станицей парсера
using System.Text.Json.Serialization;

namespace Domain.Modules.SteamIntegration.Application.DTOs.Responses
{
    public record class SteamParseResponse
    {
        [JsonPropertyName("SteamParser")]
        public SteamParser SteamParser { get; set; }
    }

    public record class SteamParser
    {
        [JsonPropertyName("StartPage")]
        public int StartPage { get; set; }

    }
}
