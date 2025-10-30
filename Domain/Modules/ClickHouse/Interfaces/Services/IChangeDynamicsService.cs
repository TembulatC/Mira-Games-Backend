using Domain.Modules.ClickHouse.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Modules.ClickHouse.Interfaces.Services
{
    /// <summary>
    /// Интерфейс сервиса для работы с динамикой изменений статистики в ClickHouse
    /// Определяет контракт для записи аналитических данных для последующего анализа трендов
    /// </summary>
    public interface IChangeDynamicsService
    {
        /// <summary>
        /// Добавляет запись о динамике изменений статистики игр
        /// Сохраняет снимок данных в ClickHouse для отслеживания изменений во времени
        /// </summary>
        /// <param name="statistic">DTO объект с данными статистики для сохранения</param>
        /// <returns>Task, представляющий асинхронную операцию сохранения</returns>
        Task AddChangeDynamic(ClickHouseStatisticDto statistic);

        /// <summary>
        /// Получает записи о динамике изменений статистики игр за указанный период
        /// Извлекает снимки данных из ClickHouse для анализа трендов во времени
        /// </summary>
        /// <param name="date">Дата в формате "YYYY-MM" для фильтрации данных</param>
        /// <returns>Список DTO объектов с данными статистики за указанный период</returns>
        Task<List<ClickHouseStatisticDto>> GetChangeDynamic(string date);
    }
}
