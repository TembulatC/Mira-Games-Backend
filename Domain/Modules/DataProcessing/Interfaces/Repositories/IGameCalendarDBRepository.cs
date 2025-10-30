using Domain.Modules.DataProcessing.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Modules.DataProcessing.Interfaces.Repositories
{
    /// <summary>
    /// Интерфейс репозитория для работы с календарем игр в базе данных
    /// </summary>
    public interface IGameCalendarDBRepository
    {
        /// <summary>
        /// Получает список игр по указанному году и месяцу
        /// </summary>
        Task<List<GameDataDBDto>> GetGamesByMonth(int year, int month);

        /// <summary>
        /// Получает количество игр по дням календаря для указанного месяца
        /// </summary>
        Task<List<CalendarDto>> GetCountGamesByCalendar(int year, int month);

        /// <summary>
        /// Получает игры по фильтру жанра и поддерживаемых платформ
        /// </summary>
        Task<List<GameDataDBDto>> GetGamesByFilter(string genre, string supportPlatforms);
    }
}
