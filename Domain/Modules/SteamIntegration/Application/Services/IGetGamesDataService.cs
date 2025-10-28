using Domain.Modules.SteamIntegration.Application.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Modules.SteamIntegration.Application.Services
{
    public interface IGetGamesDataService
    {
        Task<List<GameDataResponse>> GetGamesDataFromAPI(List<int> appid);
    }
}
