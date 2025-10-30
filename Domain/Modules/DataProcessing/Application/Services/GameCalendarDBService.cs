using Domain.Modules.DataProcessing.Application.DTOs;
using Domain.Modules.DataProcessing.Interfaces.Repositories;
using Domain.Modules.DataProcessing.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Modules.DataProcessing.Application.Services
{
    /// <summary>
    /// Сервис для работы с календарем игр
    /// </summary>
    public class GameCalendarDBService : IGameCalendarDBService
    {
        private readonly IGameCalendarDBRepository _gameCalendarDBRepository;

        public GameCalendarDBService(IGameCalendarDBRepository gameCalendarDBRepository)
        {
            _gameCalendarDBRepository = gameCalendarDBRepository;
        }

        /// <summary>
        /// Получает список игр по указанной дате в формате YYYY-MM
        /// </summary>
        public async Task<List<GameDataDBDto>> GetGamesByMonth(string date)
        {
            // Парсим "YYYY-MM" формат на год и месяц
            string[] parts = date.Split('-');
            int year = int.Parse(parts[0]);
            int month = int.Parse(parts[1]);

            return await _gameCalendarDBRepository.GetGamesByMonth(year, month);
        }

        /// <summary>
        /// Получает количество игр по дням календаря для указанной даты в формате YYYY-MM
        /// </summary>
        public async Task<List<CalendarDto>> GetCountGamesByCalendar(string date)
        {
            // Парсим "YYYY-MM" формат на год и месяц
            string[] parts = date.Split('-');
            int year = int.Parse(parts[0]);
            int month = int.Parse(parts[1]);

            return await _gameCalendarDBRepository.GetCountGamesByCalendar(year, month);
        }

        /// <summary>
        /// Получает игры по фильтру жанра и поддерживаемых платформ
        /// </summary>
        public async Task<List<GameDataDBDto>> GetGamesByFilter(string genre, string supportPlatforms)
        {
            return await _gameCalendarDBRepository.GetGamesByFilter(genre, supportPlatforms);
        }
    }
}
