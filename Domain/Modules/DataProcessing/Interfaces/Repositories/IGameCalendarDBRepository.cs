using Domain.Modules.DataProcessing.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Modules.DataProcessing.Interfaces.Repositories
{
    public interface IGameCalendarDBRepository
    {
        Task<List<GameDataDBDto>> GetGamesByMonth(int year, int month);

        Task<List<CalendarDto>> GetCountGamesByCalendar(int year, int month);

        Task<List<GameDataDBDto>> GetGamesByFilter(string genre, string supportPlatforms);
    }
}
