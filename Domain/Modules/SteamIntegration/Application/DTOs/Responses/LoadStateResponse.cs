namespace Domain.Modules.SteamIntegration.Application.DTOs.Responses
{
    public record class LoadStateResponse
    {
        public SteamParser SteamParser { get; set; }
    }

    public record class SteamParser
    {
        public int StartPage { get; set; }

    }
}
