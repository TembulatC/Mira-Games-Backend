using Domain.Modules.DataProcessing.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Modules.DataProcessing.Interfaces.Services
{
    /// <summary>
    /// Сервис для работы со статистикой игр
    /// Предоставляет методы для получения аналитических данных о популярности игровых жанров
    /// </summary>
    public interface IGameStatisticService
    {
        /// <summary>
        /// Получает список самых популярных жанров игр за указанную дату
        /// </summary>
        /// <param name="date">Дата для анализа в строковом формате</param>
        /// <returns>Список жанров с количеством игр, отсортированный по популярности</returns>
        Task<List<StatisticDto>> GetMostPopularGenres(string date);
    }
}
