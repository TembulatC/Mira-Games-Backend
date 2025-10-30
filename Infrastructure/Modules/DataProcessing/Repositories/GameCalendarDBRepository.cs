using Domain.Modules.DataProcessing.Application.DTOs;
using Domain.Modules.DataProcessing.Interfaces.Repositories;
using Domain.Modules.DataProcessing.Models;
using Infrastructure.Shared.Database.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Modules.DataProcessing.Repositories
{
    /// <summary>
    /// Репозиторий для работы с календарем игр в базе данных
    /// </summary>
    public class GameCalendarDBRepository : IGameCalendarDBRepository
    {
        private readonly AppDBContext _appDBContext;

        public GameCalendarDBRepository(AppDBContext appDBContext)
        {
            _appDBContext = appDBContext;
        }

        /// <summary>
        /// Получает список игр по указанному году и месяцу
        /// </summary>
        public async Task<List<GameDataDBDto>> GetGamesByMonth(int year, int month)
        {
            List<GameDataDBDto> releasesByMonthList = new List<GameDataDBDto>();
            List<Game> releases = await _appDBContext.Games
                .Where(game => game.ReleaseDate.Year == year && game.ReleaseDate.Month == month)
                .ToListAsync();

            // Проверяем что есть результаты
            if (!releases.Any())
                return new List<GameDataDBDto>(); // Возвращаем пустой список вместо исключения

            foreach (Game game in releases)
            {
                GameDataDBDto data = new GameDataDBDto
                {
                    Title = game.Title,
                    ReleaseDate = game.ReleaseDate,
                    Genres = game.Genres,
                    StoreURL = game.StoreURL,
                    ImageURL = game.ImageURL,
                    ShortDescription = game.ShortDescription,
                    SupportedPlatforms = game.SupportedPlatforms
                };

                releasesByMonthList.Add(data);
            }

            return releasesByMonthList;
        }

        /// <summary>
        /// Получает количество игр по дням календаря для указанного месяца
        /// </summary>
        public async Task<List<CalendarDto>> GetCountGamesByCalendar(int year, int month)
        {
            // Создаем словарь с количеством игр по дням из базы данных
            Dictionary<DateOnly, int> releasesCountByDays = await _appDBContext.Games
                .Where(days => days.ReleaseDate.Year == year && days.ReleaseDate.Month == month)
                .GroupBy(game => game.ReleaseDate)
                .Select(group => new { Date = group.Key, Count = group.Count() })
                .ToDictionaryAsync(x => x.Date, x => x.Count);

            // Определяем сколько дней в указанном месяце (учитывая високосные годы)
            int daysInMonth = DateTime.DaysInMonth(year, month);

            // Создаем список всех дней месяца от 1 до последнего дня
            List<DateOnly> allDaysInMonth = Enumerable.Range(1, daysInMonth) // Ноябрь = 30 дней
                .Select(day => new DateOnly(year, month, day))
                .ToList();

            // Создаем массив объектов Days для каждого дня месяца
            var daysWithCounts = allDaysInMonth
                .Select(date => new Days
                {
                    date = date,
                    count = releasesCountByDays.GetValueOrDefault(date, 0)
                })
                .ToArray();

            // Создаем список для результата
            List<CalendarDto> releasesByDaysList = new List<CalendarDto>
            {
                new CalendarDto
                {
                    month = $"{year}-{month:D2}",
                    days = daysWithCounts
                }
            };

            return releasesByDaysList;
        }

        /// <summary>
        /// Получает игры по фильтру жанра и поддерживаемых платформ
        /// </summary>
        public async Task<List<GameDataDBDto>> GetGamesByFilter(string genre, string supportPlatforms)
        {
            List<GameDataDBDto> gamesFilterList = new List<GameDataDBDto>();

            List<Game> releases = await _appDBContext.Games
                .Where(game => game.Genres.Contains(genre) == true && game.SupportedPlatforms.Contains(supportPlatforms) == true)
                .ToListAsync();

            // Проверяем что есть результаты
            if (!releases.Any())
                return new List<GameDataDBDto>(); // Возвращаем пустой список вместо исключения

            foreach (Game game in releases)
            {
                GameDataDBDto data = new GameDataDBDto
                {
                    Title = game.Title,
                    ReleaseDate = game.ReleaseDate,
                    Genres = game.Genres,
                    StoreURL = game.StoreURL,
                    ImageURL = game.ImageURL,
                    ShortDescription = game.ShortDescription,
                    SupportedPlatforms = game.SupportedPlatforms
                };

                gamesFilterList.Add(data);
            }

            return gamesFilterList;
        }
    }
}
