using Domain.Modules.DataProcessing.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Modules.DataProcessing.Interfaces.Repositories
{
    /// <summary>
    /// Репозиторий для работы с данными игр в базе данных
    /// Обеспечивает абстракцию над операциями чтения/записи игровых данных
    /// </summary>
    public interface IGameDataDBRepository
    {
        /// <summary>
        /// Загружает данные игр из JSON файла
        /// </summary>
        /// <returns>Список DTO с данными игр или null если файл не существует</returns>
        Task<List<GameDataDBDto>?> LoadGameData();

        /// <summary>
        /// Добавляет новые и обновляет существующие данные игр в базе данных
        /// </summary>
        /// <param name="gameDataDBOptions">Список DTO с данными для сохранения</param>
        /// <exception cref="ArgumentException">Выбрасывается если передан null список</exception>
        Task AddAndUpdateGamesData(List<GameDataDBDto>? gameDataDBOptions);
    }
}
