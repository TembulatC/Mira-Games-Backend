using Domain.Modules.Orchestrator.UseCases;
using Domain.Modules.SteamIntegration.Application.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using MiraGamesBackend.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace MiraGamesBackend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MainController : Controller
    {
        private readonly GetGamesDataUseCase _addGamesDataUseCase;

        public MainController(GetGamesDataUseCase addGamesDataUseCase)
        {
            _addGamesDataUseCase = addGamesDataUseCase;
        }

        [HttpPost]
        [Route("[action]")]
        [SwaggerOperation(Summary = SwaggerTexts.GetGames.Summary,
            Description = SwaggerTexts.GetGames.Description)]
        public async Task<IActionResult> GetGames()
        {
            List<GameDataResponse> listID = await _addGamesDataUseCase.GetGames();
            return Json(listID);
        }
    }
}
