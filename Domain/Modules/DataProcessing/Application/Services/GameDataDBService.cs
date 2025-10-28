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
    /// Реализация сервиса для управления данными игр
    /// Координирует процесс загрузки и сохранения игровых данных
    /// </summary>
    public class GameDataDBService : IGameDataDBService
    {
        private readonly IGameDataDBRepository _gameDataDBRepository;

        /// <summary>
        /// Инициализирует новый экземпляр сервиса
        /// </summary>
        /// <param name="gameDataDBRepository">Репозиторий для работы с данными игр</param>
        public GameDataDBService(IGameDataDBRepository gameDataDBRepository)
        {
            _gameDataDBRepository = gameDataDBRepository;
        }

        /// <summary>
        /// Выполняет полный цикл синхронизации данных игр:
        /// 1. Загружает данные из JSON файла
        /// 2. Сохраняет/обновляет данные в базе данных
        /// </summary>
        public async Task AddAndUpdateGamesData()
        {
            List<GameDataDBDto>? gameDataDB = await _gameDataDBRepository.LoadGameData();
            await _gameDataDBRepository.AddAndUpdateGamesData(gameDataDB);
        }
    }
}
