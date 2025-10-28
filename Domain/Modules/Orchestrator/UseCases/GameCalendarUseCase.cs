using Domain.Modules.DataProcessing.Application.DTOs;
using Domain.Modules.DataProcessing.Interfaces.Repositories;
using Domain.Modules.DataProcessing.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Modules.Orchestrator.UseCases
{
    public class GameCalendarUseCase
    {
        private readonly IGameCalendarDBService _gameCalendarDBService;

        public GameCalendarUseCase(IGameCalendarDBService gameCalendarDBService)
        {
            _gameCalendarDBService = gameCalendarDBService;
        }

        public async Task<List<GameDataDBDto>> GetGamesByMonth(string date)
        {
            return await _gameCalendarDBService.GetGamesByMonth(date);
        }

        public async Task<List<CalendarDto>> GetCountGamesByCalendar(string date)
        {
            return await _gameCalendarDBService.GetCountGamesByCalendar(date);
        }

        public async Task<List<GameDataDBDto>> GetGamesByFilter(string genre, string supportPlatforms)
        {
            return await _gameCalendarDBService.GetGamesByFilter(genre, supportPlatforms);
        }
    }
}
