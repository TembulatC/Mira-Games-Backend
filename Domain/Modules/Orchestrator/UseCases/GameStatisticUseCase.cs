using Domain.Modules.ClickHouse.Application.DTOs;
using Domain.Modules.ClickHouse.Interfaces.Services;
using Domain.Modules.DataProcessing.Application.DTOs;
using Domain.Modules.DataProcessing.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Modules.Orchestrator.UseCases
{
    /// <summary>
    /// Use case для работы со статистикой игр
    /// Координирует взаимодействие между сервисами статистики и динамики изменений
    /// </summary>
    public class GameStatisticUseCase
    {
        private readonly IGameStatisticService _gameStatisticService;
        private readonly IChangeDynamicsService _changeDynamicsService;

        public GameStatisticUseCase(IGameStatisticService gameStatisticService, IChangeDynamicsService changeDynamicsService)
        {
            _gameStatisticService = gameStatisticService;
            _changeDynamicsService = changeDynamicsService;
        }

        /// <summary>
        /// Получает самые популярные жанры игр за указанную дату
        /// </summary>
        /// <param name="date">Дата в формате "YYYY-MM"</param>
        /// <returns>Список жанров с количеством игр</returns>
        public async Task<List<StatisticDto>> GetMostPopularGenres(string date)
        {
            return await _gameStatisticService.GetMostPopularGenres(date);
        }

        /// <summary>
        /// Добавляет запись о динамике изменений статистики в следующем месяце
        /// Автоматически формирует данные за следующий месяц и сохраняет в ClickHouse
        /// </summary>
        /// <returns>Task, представляющий асинхронную операцию</returns>
        public async Task AddChangeDynamic()
        {
            // Формируем дату для статистики
            DateTime todayDateTime = DateTime.Today;
            DateOnly today = DateOnly.FromDateTime(todayDateTime);

            string formatString = today.AddMonths(1).ToString("yyyy-MM");

            List<StatisticDto> statistics = await _gameStatisticService.GetMostPopularGenres(formatString);

            ClickHouseStatisticDto clickHouseStatistic = new ClickHouseStatisticDto
            {
                date = DateTime.UtcNow, // В ClickHouse сохраняется подробное время снимка
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

        /// <summary>
        /// Получает записи о динамике изменений статистики игр за указанный период
        /// Извлекает исторические данные из ClickHouse для анализа трендов
        /// </summary>
        /// <param name="date">Дата в формате "YYYY-MM" для фильтрации данных</param>
        /// <returns>Список DTO объектов с данными статистики за указанный период</returns>
        public async Task<List<ClickHouseStatisticDto>> GetChangeDynamic(string date)
        {
            return await _changeDynamicsService.GetChangeDynamic(date);
        }
    }
}
