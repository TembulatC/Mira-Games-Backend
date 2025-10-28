using Domain.Modules.SteamIntegration.Application.DTOs;
using Domain.Modules.SteamIntegration.Application.DTOs.Responses;

namespace Domain.Modules.SteamIntegration.Interfaces.Repositories
{
    public interface ISteamParseRepository
    {
        /// <summary>
        /// Загружает состояние парсера и дургих данных
        /// </summary>
        Task<SteamParseResponse?> LoadState();
        
        /// <summary>
        /// Парсит ID игр с ноябрьскими релизами 2025 года из Steam
        /// </summary>
        Task<List<int>> ParseSteamGamesId(SteamParseDto parseOptions);

        /// <summary>
        /// Сохраняет состояние парсера и других данных
        /// </summary>
        Task SaveState(SteamParseResponse saveStateResponse);
    }
}
