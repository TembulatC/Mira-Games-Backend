using Domain.Modules.SteamIntegration.Application.DTOs.Responses;
using Domain.Modules.SteamIntegration.Interfaces.Repositories;
using Domain.Modules.SteamIntegration.Interfaces.Services;

namespace Domain.Modules.SteamIntegration.Application.Services
{
    /// <summary>
    /// Сервис для получения и сохранения данных об играх из Steam API
    /// Координирует работу с репозиторием Steam API
    /// </summary>
    public class GetSteamAPIService : IGetSteamAPIService
    {
        private readonly ISteamAPIRepository _steamAPIRepository;

        /// <summary>
        /// Инициализирует новый экземпляр сервиса для работы с данными игр
        /// </summary>
        /// <param name="steamAPIRepository">Репозиторий для взаимодействия с Steam API</param>
        public GetSteamAPIService(ISteamAPIRepository steamAPIRepository)
        {
            _steamAPIRepository = steamAPIRepository;
        }

        /// <summary>
        /// Получает данные об играх из Steam API и сохраняет их в JSON файл
        /// Выполняет два основных действия:
        /// 1. Получение данных об играх по списку AppID через Steam API
        /// 2. Сохранение отфильтрованных данных (ноябрьские релизы) в JSON файл
        /// </summary>
        /// <param name="appid">Список идентификаторов игр (AppID) для получения данных</param>
        /// <returns>Список объектов с полными данными об играх из Steam API</returns>
        public async Task<List<SteamAPIResponse>> GetGamesDataFromAPI(List<int> appid)
        {
            // Получение данных об играх из Steam API
            List<SteamAPIResponse> gameDataResponses = await _steamAPIRepository.GetGamesData(appid);

            // Сохранение отфильтрованных данных в JSON файл
            await _steamAPIRepository.SaveGamesData(gameDataResponses);
            return gameDataResponses;
        }
    }
}
