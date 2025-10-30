using Domain.Modules.DataProcessing.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Modules.DataProcessing.Interfaces.Services
{
    /// <summary>
    /// Интерфейс сервиса для работы с календарем игр
    /// </summary>
    public interface IGameCalendarDBService
    {
        /// <summary>
        /// Получает список игр по указанной дате в формате YYYY-MM
        /// </summary>
        Task<List<GameDataDBDto>> GetGamesByMonth(string date);

        /// <summary>
        /// Получает количество игр по дням календаря для указанной даты в формате YYYY-MM
        /// </summary>
        Task<List<CalendarDto>> GetCountGamesByCalendar(string date);

        /// <summary>
        /// Получает игры по фильтру жанра и поддерживаемых платформ
        /// </summary>
        Task<List<GameDataDBDto>> GetGamesByFilter(string genre, string supportPlatforms);
    }
}
