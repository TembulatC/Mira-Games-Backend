using Domain.Modules.DataProcessing.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Modules.DataProcessing.Interfaces.Services
{
    public interface IGameCalendarDBService
    {
        Task<List<GameDataDBDto>> GetGamesByMonth(string date);

        Task<List<CalendarDto>> GetCountGamesByCalendar(string date);

        Task<List<GameDataDBDto>> GetGamesByFilter(string genre, string supportPlatforms);
    }
}
