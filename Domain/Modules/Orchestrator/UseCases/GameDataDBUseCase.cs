using Domain.Modules.DataProcessing.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Modules.Orchestrator.UseCases
{
    /// <summary>
    /// Use Case для управления данными игр в базе данных
    /// Представляет собой точку входа для операций с игровыми данными
    /// </summary>
    public class GameDataDBUseCase
    {
        private readonly IGameDataDBService _gameDataDBService;

        /// <summary>
        /// Инициализирует новый экземпляр Use Case
        /// </summary>
        /// <param name="gameDataDBService">Сервис для работы с данными игр</param>
        public GameDataDBUseCase(IGameDataDBService gameDataDBService)
        {
            _gameDataDBService = gameDataDBService;
        }

        /// <summary>
        /// Выполняет операцию добавления и обновления данных игр
        /// Координирует вызов соответствующего сервиса
        /// </summary>
        public async Task AddAndUpdateGamesData()
        {
            await _gameDataDBService.AddAndUpdateGamesData();
        }
    }
}
