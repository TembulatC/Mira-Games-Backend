using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Modules.SteamIntegration.Application.PipeLines
{
    public interface IGetGamesIDPipeLine
    {
        Task<List<int>> GetGamesId();
    }
}
