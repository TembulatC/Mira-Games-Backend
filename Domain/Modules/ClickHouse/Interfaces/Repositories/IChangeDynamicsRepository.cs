using Domain.Modules.ClickHouse.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Modules.ClickHouse.Interfaces.Repositories
{
    /// <summary>
    /// Интерфейс репозитория для работы с динамикой изменений в ClickHouse
    /// Определяет контракт для доступа к данным и записи статистической информации
    /// </summary>
    public interface IChangeDynamicsRepository
    {
        /// <summary>
        /// Добавляет запись о динамике изменений статистики игр
        /// Выполняет вставку данных в таблицу ClickHouse для последующего анализа
        /// </summary>
        /// <param name="statistic">DTO объект с данными статистики для сохранения</param>
        /// <returns>Task, представляющий асинхронную операцию сохранения</returns>
        Task AddChangeDynamic(ClickHouseStatisticDto statistic);

        Task<List<ClickHouseStatisticDto>> GetChangeDynamic(string date);
    }
}
