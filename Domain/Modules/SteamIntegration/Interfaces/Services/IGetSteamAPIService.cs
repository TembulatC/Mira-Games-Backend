using Domain.Modules.SteamIntegration.Application.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Modules.SteamIntegration.Interfaces.Services
{
    /// <summary>
    /// Интерфейс сервиса для получения и сохранения данных об играх из Steam API
    /// Определяет контракт для координации работы с репозиторием Steam API
    /// </summary>
    public interface IGetSteamAPIService
    {
        /// <summary>
        /// Получает данные об играх из Steam API и сохраняет их в JSON файл
        /// Выполняет два основных действия:
        /// 1. Получение данных об играх по списку AppID через Steam API
        /// 2. Сохранение отфильтрованных данных (ноябрьские релизы) в JSON файл
        /// </summary>
        /// <param name="appid">Список идентификаторов игр (AppID) для получения данных</param>
        /// <returns>Список объектов с полными данными об играх из Steam API</returns>
        Task<List<SteamAPIResponse>> GetGamesDataFromAPI(List<int> appid);
    }
}
