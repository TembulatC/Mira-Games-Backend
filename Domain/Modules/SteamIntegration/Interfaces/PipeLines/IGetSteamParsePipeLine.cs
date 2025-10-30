using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Modules.SteamIntegration.Interfaces.PipeLines
{
    /// <summary>
    /// Интерфейс пайплайна для получения ID игр с ноябрьскими релизами из Steam
    /// Определяет контракт для управления процессом парсинга: загрузка состояния → парсинг → сохранение состояния
    /// </summary>
    public interface IGetSteamParsePipeLine
    {
        /// <summary>
        /// Получает ID игр с ноябрьскими релизами через парсинг Steam
        /// Выполняет полный цикл: загрузка состояния → парсинг → сохранение состояния
        /// </summary>
        /// <returns>Список ID игр с релизами в ноябре 2025 года</returns>
        Task<List<int>> GetGamesId();
    }
}
