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

        Task<List<ClickHouseStatisticDto>> GetChangeDynamic(string date);
    }
}
