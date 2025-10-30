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
    /// Репозиторий для работы со статистикой игр
    /// Обеспечивает доступ к данным о играх и их статистике
    /// </summary>
    public class GameStatisticRepository : IGameStatisticRepository
    {
        private readonly AppDBContext _appDBContext;

        public GameStatisticRepository(AppDBContext appDBContext)
        {
            _appDBContext = appDBContext;
        }

        /// <summary>
        /// Получает список самых популярных жанров за указанный год и месяц
        /// </summary>
        /// <param name="year">Год для фильтрации</param>
        /// <param name="month">Месяц для фильтрации</param>
        /// <returns>Список из 5 самых популярных жанров с количеством игр в каждом</returns>
        public async Task<List<StatisticDto>> GetMostPopularGenres(int year, int month)
        {
            List<StatisticDto> genreStats = await _appDBContext.Games
                .Where(game => game.ReleaseDate.Year == year && game.ReleaseDate.Month == month)
                .SelectMany(game => game.Genres)  // "Разворачиваем" списки жанров каждой игры в плоскую структуру
                .GroupBy(genre => genre)
                .Select(group => new StatisticDto // Преобразуем группы в объекты StatisticDto
                {
                    genre = group.Key,
                    games = group.Count()
                })
                .OrderByDescending(x => x.games) // Сортируем по убыванию количества игр
                .Take(5) // Берем только первые 5 жанров
                .ToListAsync();

            return genreStats;
        }
    }
}
