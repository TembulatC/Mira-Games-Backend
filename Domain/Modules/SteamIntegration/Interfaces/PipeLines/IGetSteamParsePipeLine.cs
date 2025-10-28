using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Modules.SteamIntegration.Interfaces.PipeLines
{
    public interface IGetSteamParsePipeLine
    {
        Task<List<int>> GetGamesId();
    }
}
