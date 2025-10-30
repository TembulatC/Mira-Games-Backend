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
    /// Сервис для работы со статистикой игр
    /// </summary>
    public class GameStatisticService : IGameStatisticService
    {
        private readonly IGameStatisticRepository _gameStatisticRepository;

        public GameStatisticService(IGameStatisticRepository gameStatisticRepository)
        {
            _gameStatisticRepository = gameStatisticRepository;
        }

        /// <summary>
        /// Получает самые популярные жанры за указанную дату
        /// </summary>
        /// <param name="date">Дата в формате "YYYY-MM"</param>
        /// <returns>Список жанров с количеством игр</returns>
        public async Task<List<StatisticDto>> GetMostPopularGenres(string date)
        {
            // Парсим "YYYY-MM" формат на год и месяц
            string[] parts = date.Split('-');
            int year = int.Parse(parts[0]);
            int month = int.Parse(parts[1]);

            return await _gameStatisticRepository.GetMostPopularGenres(year, month);
        }
    }
}
