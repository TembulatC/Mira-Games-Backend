using Domain.Modules.Orchestrator.UseCases;
using Domain.Modules.SteamIntegration.Application.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using MiraGamesBackend.Utilities;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MiraGamesBackend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MainController : Controller
    {
        private readonly GetGamesDataUseCase _addGamesDataUseCase;
        private readonly GameDataDBUseCase _gameDataDBUseCase;
        private readonly GameCalendarUseCase _gameCalendarUseCase;

        public MainController(GetGamesDataUseCase addGamesDataUseCase, GameDataDBUseCase gameDataDBUseCase, GameCalendarUseCase gameCalendarUseCase)
        {
            _addGamesDataUseCase = addGamesDataUseCase;
            _gameDataDBUseCase = gameDataDBUseCase;
            _gameCalendarUseCase = gameCalendarUseCase;
        }

        [HttpPost]
        [Route("[action]")]
        [SwaggerOperation(Summary = SwaggerTexts.GetGames.Summary,
            Description = SwaggerTexts.GetGames.Description)]
        public async Task<IActionResult> GetGames()
        {
            List<SteamAPIResponse> listID = await _addGamesDataUseCase.GetGames(); // Выполняем Use Case для парсинга данных в gamesinfo.json
            return Json(listID); // Возвращаем JSON
        }

        [HttpPost]
        [Route("[action]")]
        [SwaggerOperation(Summary = SwaggerTexts.AddAndUpdateGamesData.Summary,
            Description = SwaggerTexts.AddAndUpdateGamesData.Description)]
        public async Task<IActionResult> AddAndUpdateGamesData()
        {
            try
            {                
                await _gameDataDBUseCase.AddAndUpdateGamesData(); // Выполняем Use Case для обработки игровых данных
                return NoContent(); // Возвращаем 204 No Content - операция выполнена успешно, тело ответа не требуется
            }
            catch (Exception ex)
            {                
                return BadRequest(ex.Message); // Возвращаем 400 Bad Request с сообщением об ошибке
            }
        }

        [HttpGet]
        [Route("games")]
        [SwaggerOperation(Summary = SwaggerTexts.GetGamesByMonth.Summary,
            Description = SwaggerTexts.GetGamesByMonth.Description)]
        public async Task<IActionResult> GetGamesByMonth([FromQuery] string month)
        {
            // Проверяем базовый формат
            if (!Regex.IsMatch(month, @"^\d{4}-\d{2}$"))
                return BadRequest("Неверный формат. Используйте следующий формат: YYYY-MM");

            try
            {
                var games = await _gameCalendarUseCase.GetGamesByMonth(month);
                return Json(games);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("games/calendar")]
        public async Task<IActionResult> GetCountGamesByCalendar([FromQuery] string month)
        {
            // Проверяем базовый формат
            if (!Regex.IsMatch(month, @"^\d{4}-\d{2}$"))
                return BadRequest("Неверный формат. Используйте следующий формат: YYYY-MM");

            try
            {
                var games = await _gameCalendarUseCase.GetCountGamesByCalendar(month);
                return Json(games);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("games/filter")]
        public async Task<IActionResult> GetGamesByFilter([FromQuery] [SwaggerParameter("Выберите жанр игры", Required = true)] string genre, [SwaggerParameter("Выберите платфому", Required = true)] string supportPlatforms)
        {
            try
            {
                string genreString = genre.ToString();
                string supportPlatformsString = supportPlatforms.ToString();
                var gamesByFilter = await _gameCalendarUseCase.GetGamesByFilter(genreString, supportPlatformsString);
                return Json(gamesByFilter);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
