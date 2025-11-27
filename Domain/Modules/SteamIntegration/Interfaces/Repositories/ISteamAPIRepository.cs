using Domain.Modules.SteamIntegration.Application.DTOs.Responses;

namespace Domain.Modules.SteamIntegration.Interfaces.Repositories
{
    public interface ISteamAPIRepository
    {
        /// <summary>
        /// Получает данные о будущих релизах из Steam API по Id
        /// </summary>
        Task<List<SteamAPIResponse>> GetGamesData(List<int> appId);

        /// <summary>
        /// Сохраняет данные о будущих релизах из Steam API в gameinfo.json
        /// </summary>
        Task SaveGamesData(List<SteamAPIResponse> gameDataResponses);
    }
}
