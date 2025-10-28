using Domain.Modules.DataProcessing.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Modules.DataProcessing.Interfaces.Services
{
    /// <summary>
    /// Сервис для управления данными игр в базе данных
    /// Инкапсулирует бизнес-логику работы с игровыми данными
    /// </summary>
    public interface IGameDataDBService
    {
        /// <summary>
        /// Выполняет полный цикл обновления данных игр:
        /// загрузка из JSON файла → сохранение в базу данных
        /// </summary>
        Task AddAndUpdateGamesData();
    }
}
