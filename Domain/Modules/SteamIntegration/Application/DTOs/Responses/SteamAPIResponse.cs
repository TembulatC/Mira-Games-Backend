// DTO для десериализации данных из SteamAPI
using System.Text.Json.Serialization;

namespace Domain.Modules.SteamIntegration.Application.DTOs.Responses
{
    public record class SteamAPIResponse
    {
        [JsonPropertyName("success")]
        public bool success { get; set; }

        [JsonPropertyName("data")]
        public Data? data { get; set; }
    }

    public record class Data
    {
        [JsonPropertyName("type")]
        public string type { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string name { get; set; } = string.Empty;

        [JsonPropertyName("steam_appid")]
        public required int appid { get; set; }

        [JsonPropertyName("short_description")]
        public string shortDescription { get; set; } = string.Empty;

        [JsonPropertyName("header_image")]
        public string imageURL { get; set; } = string.Empty;

        [JsonPropertyName("platforms")]
        public SupportedPlatforms? supportedPlatforms { get; set; }

        [JsonPropertyName("genres")]
        public Genre[]? genres { get; set; } = null;

        [JsonPropertyName("release_date")]
        public ReleaseDate? releaseDate { get; set; }
    }

    public record class SupportedPlatforms
    {
        [JsonPropertyName("windows")]
        public bool windows { get; set; }

        [JsonPropertyName("mac")]
        public bool mac { get; set; }

        [JsonPropertyName("linux")]
        public bool linux { get; set; }
    }

    public record class Genre
    {
        [JsonPropertyName("description")]
        public string description { get; set; } = string.Empty;
    }

    public record class ReleaseDate
    {
        [JsonPropertyName("coming_soon")]
        public bool coming_soon { get; set; }

        [JsonPropertyName("date")]
        public string date { get; set; } = string.Empty;
    }
}
