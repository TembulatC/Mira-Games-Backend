using Domain.Modules.SteamIntegration.Application.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Modules.SteamIntegration.Interfaces.Services
{
    public interface IGetSteamAPIService
    {
        Task<List<SteamAPIResponse>> GetGamesDataFromAPI(List<int> appid);
    }
}
