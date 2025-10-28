using Domain.Modules.SteamIntegration.Application.DTOs.Options;
using Domain.Modules.SteamIntegration.Application.DTOs.Responses;

namespace Domain.Modules.SteamIntegration.Interfaces
{
    public interface ISteamParseRepository
    {
        /// <summary>
        /// Загружает состояние парсера и дургих данных
        /// </summary>
        Task<LoadStateResponse?> LoadState();
        
        /// <summary>
        /// Парсит ID игр с ноябрьскими релизами 2025 года из Steam
        /// </summary>
        Task<List<int>> ParseSteamGamesId(LoadStateOptions parseOptions);

        /// <summary>
        /// Сохраняет состояние парсера и других данных
        /// </summary>
        Task SaveState(LoadStateResponse saveStateResponse);
    }
}
