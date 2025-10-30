using Domain.Modules.ClickHouse.Application.DTOs;
using Domain.Modules.ClickHouse.Interfaces.Repositories;
using Domain.Modules.ClickHouse.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Modules.ClickHouse.Application.Services
{
    /// <summary>
    /// Сервис для работы с динамикой изменений в ClickHouse
    /// Реализует бизнес-логику работы со статистическими данными
    /// </summary>
    public class ChangeDynamicsService : IChangeDynamicsService
    {
        private readonly IChangeDynamicsRepository _changeDynamicsRepository;

        public ChangeDynamicsService(IChangeDynamicsRepository changeDynamicsRepository)
        {
            _changeDynamicsRepository = changeDynamicsRepository;
        }

        /// <summary>
        /// Добавляет запись о динамике изменений статистики игр
        /// Делегирует выполнение операции репозиторию для сохранения в ClickHouse
        /// </summary>
        /// <param name="statistic">DTO объект с данными статистики для сохранения</param>
        /// <returns>Task, представляющий асинхронную операцию сохранения</returns>
        public async Task AddChangeDynamic(ClickHouseStatisticDto statistic)
        {
            await _changeDynamicsRepository.AddChangeDynamic(statistic);
        }

        public async Task<List<ClickHouseStatisticDto>> GetChangeDynamic(string date)
        {
            return await _changeDynamicsRepository.GetChangeDynamic(date);
        }
    }
}
