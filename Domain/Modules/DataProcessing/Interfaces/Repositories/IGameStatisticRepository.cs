using Domain.Modules.DataProcessing.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Modules.DataProcessing.Interfaces.Repositories
{
    /// <summary>
    /// Репозиторий для работы со статистикой игр
    /// </summary>
    public interface IGameStatisticRepository
    {
        /// <summary>
        /// Получает самые популярные жанры за указанный год и месяц
        /// </summary>
        /// <param name="year">Год для анализа</param>
        /// <param name="month">Месяц для анализа</param>
        /// <returns>Список жанров с количеством игр</returns>
        Task<List<StatisticDto>> GetMostPopularGenres(int year, int month);
    }
}
