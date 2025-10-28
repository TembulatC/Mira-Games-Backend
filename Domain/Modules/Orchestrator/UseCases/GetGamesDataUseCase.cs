using Domain.Modules.SteamIntegration.Application.DTOs.Responses;
using Domain.Modules.SteamIntegration.Interfaces.PipeLines;
using Domain.Modules.SteamIntegration.Interfaces.Services;

namespace Domain.Modules.Orchestrator.UseCases
{
    /// <summary>
    /// UseCase для полного цикла сбора данных об играх
    /// Координирует работу пайплайнов и сервисов для получения ID игр и их детальных данных
    /// и последующего сохранения в JSON файл
    /// </summary>
    public class GetGamesDataUseCase
    {
        private readonly IGetSteamParsePipeLine _getGamesIDPipeLine;
        private readonly IGetSteamAPIService _getGamesDataService;

        /// <summary>
        /// Инициализирует новый экземпляр UseCase для добавления данных об играх
        /// </summary>
        /// <param name="getGamesIDPipeLine">Пайплайн для получения ID игр через парсинг Steam</param>
        /// <param name="getGamesDataService">Сервис для получения детальных данных из Steam API</param>
        public GetGamesDataUseCase(IGetSteamParsePipeLine getGamesIDPipeLine, IGetSteamAPIService getGamesDataService)
        {
            _getGamesIDPipeLine = getGamesIDPipeLine;
            _getGamesDataService = getGamesDataService;
        }

        /// <summary>
        /// Выполняет полный цикл сбора данных об играх:
        /// 1. Получение списка ID игр с ноябрьскими релизами через парсинг Steam страниц
        /// 2. Получение детальных данных об играх через Steam API
        /// 3. Автоматическое сохранение отфильтрованных данных в JSON файл
        /// </summary>
        /// <returns>Список объектов с полными данными об играх из Steam API</returns>
        public async Task<List<SteamAPIResponse>> GetGames()
        {
            // Этап 1: Парсинг Steam для получения ID ноябрьских игр
            List<int> appIdList = await _getGamesIDPipeLine.GetGamesId();

            // Этап 2: Парсинг SteamAPI для получения детальных данных и сохранение в JSON
            return await _getGamesDataService.GetGamesDataFromAPI(appIdList);
        }
    }
}
