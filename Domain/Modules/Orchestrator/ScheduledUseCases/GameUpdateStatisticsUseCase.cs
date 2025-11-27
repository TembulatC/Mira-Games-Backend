using Domain.Modules.ClickHouse.Application.DTOs;
using Domain.Modules.ClickHouse.Interfaces.Services;
using Domain.Modules.DataProcessing.Application.DTOs;
using Domain.Modules.DataProcessing.Application.Services;
using Domain.Modules.DataProcessing.Interfaces.Services;
using Domain.Modules.SteamIntegration.Interfaces.PipeLines;
using Domain.Modules.SteamIntegration.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Modules.Orchestrator.ScheduledUseCases
{
    /// <summary>
    /// Use case для запланированных операций обновления статистики игр
    /// Координирует выполнение периодических задач по обновлению данных
    /// </summary>
    public class GameUpdateStatisticsUseCase
    {
        private readonly IGetSteamParsePipeLine _getSteamParsePipeLine;
        private readonly IGetSteamAPIService _getSteamAPIService;
        private readonly IGameDataDBService _gameDataDBService;
        private readonly IGameStatisticService _gameStatisticService;
        private readonly IChangeDynamicsService _changeDynamicsService;

        public GameUpdateStatisticsUseCase(IGetSteamParsePipeLine getSteamParsePipeLine, IGetSteamAPIService getSteamAPIService, IGameDataDBService gameDataDBService, IGameStatisticService gameStatisticService, IChangeDynamicsService changeDynamicsService)
        {       
            _getSteamParsePipeLine = getSteamParsePipeLine;
            _getSteamAPIService = getSteamAPIService;
            _gameDataDBService = gameDataDBService;
            _gameStatisticService = gameStatisticService;
            _changeDynamicsService = changeDynamicsService;
        }

        /// <summary>
        /// Запланированное получение ID игр из Steam и запись данных в JSON
        /// Выполняет два этапа: парсинг ID игр и получение данных через Steam API
        /// </summary>
        /// <returns>Task, представляющий асинхронную операцию</returns>
        public async Task ScheduledGetGamesId()
        {
            List<int> listId = await _getSteamParsePipeLine.GetGamesId();
            
            await _getSteamAPIService.GetGamesDataFromAPI(listId);

        }

        /// <summary>
        /// Запланированное добавление и обновление данных об играх в БД
        /// </summary>
        /// <returns>Task, представляющий асинхронную операцию</returns>
        public async Task ScheduledAddAndUpdateGamesData()
        {
            await _gameDataDBService.AddAndUpdateGamesData();
        }

        /// <summary>
        /// Запланированное добавление записи о динамике изменений статистики
        /// Сохраняет статистику на следующий месяц в ClickHouse
        /// </summary>
        /// <returns>Task, представляющий асинхронную операцию</returns>
        public async Task ScheduledAddChangeDynamic()
        {
            // Формируем дату
            DateTime todayDateTime = DateTime.Today;
            DateOnly today = DateOnly.FromDateTime(todayDateTime);

            string formatString = today.AddMonths(1).ToString("yyyy-MM");

            Console.WriteLine(formatString);

            List<StatisticDto> statistics = await _gameStatisticService.GetMostPopularGenres(formatString);

            ClickHouseStatisticDto clickHouseStatistic = new ClickHouseStatisticDto
            {
                date = DateTime.Now, // В ClickHouse сохраняется подробное время снимка
                popularGenres = statistics.Select(statistic => new PopularGenres
                {
                    genre = statistic.genre,
                    games = statistic.games
                })
                .ToArray()
            };

            // Сохраняем динамику изменений
            await _changeDynamicsService.AddChangeDynamic(clickHouseStatistic);
        }
    }
}
