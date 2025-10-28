using Domain.Modules.SteamIntegration.Application.DTOs.Responses;

namespace Domain.Modules.SteamIntegration.Interfaces
{
    public interface ISteamAPIRepository
    {
        /// <summary>
        /// Получает данные о ноябрьских релизах из Steam API по Id
        /// </summary>
        Task<List<GameDataResponse>> GetGamesData(List<int> appId);

        /// <summary>
        /// Сохраняет данные о ноябрьских релизах из Steam API в gameinfo.json
        /// </summary>
        Task SaveGamesData(List<GameDataResponse> gameDataResponses);
    }
}
