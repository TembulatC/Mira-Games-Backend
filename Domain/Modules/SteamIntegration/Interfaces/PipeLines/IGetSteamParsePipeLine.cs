using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Modules.SteamIntegration.Interfaces.PipeLines
{
    /// <summary>
    /// Интерфейс пайплайна для получения ID релизов игр на следующий месяц из Steam
    /// Определяет контракт для управления процессом парсинга: загрузка состояния → парсинг → сохранение состояния
    /// </summary>
    public interface IGetSteamParsePipeLine
    {
        /// <summary>
        /// Получает ID игр с релизами на следующий месяц через парсинг Steam
        /// Выполняет полный цикл: загрузка состояния → парсинг → сохранение состояния
        /// </summary>
        /// <returns>Список ID игр с будущими релизами</returns>
        Task<List<int>> GetGamesId();
    }
}
