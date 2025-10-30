using Domain.Modules.DataProcessing.Application.DTOs;
using Domain.Modules.DataProcessing.Interfaces.Repositories;
using Domain.Modules.DataProcessing.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Modules.Orchestrator.UseCases
{
    /// <summary>
    /// Use case для работы с календарем игр
    /// Координирует вызовы сервисов для операций с играми
    /// </summary>
    public class GameCalendarUseCase
    {
        private readonly IGameCalendarDBService _gameCalendarDBService;

        public GameCalendarUseCase(IGameCalendarDBService gameCalendarDBService)
        {
            _gameCalendarDBService = gameCalendarDBService;
        }

        /// <summary>
        /// Получает игры по указанному месяцу
        /// </summary>
        public async Task<List<GameDataDBDto>> GetGamesByMonth(string date)
        {
            return await _gameCalendarDBService.GetGamesByMonth(date);
        }

        /// <summary>
        /// Получает количество игр по дням календаря
        /// </summary>
        public async Task<List<CalendarDto>> GetCountGamesByCalendar(string date)
        {
            return await _gameCalendarDBService.GetCountGamesByCalendar(date);
        }

        /// <summary>
        /// Получает игры по фильтру жанра и платформ
        /// </summary>
        public async Task<List<GameDataDBDto>> GetGamesByFilter(string genre, string supportPlatforms)
        {
            return await _gameCalendarDBService.GetGamesByFilter(genre, supportPlatforms);
        }
    }
}
