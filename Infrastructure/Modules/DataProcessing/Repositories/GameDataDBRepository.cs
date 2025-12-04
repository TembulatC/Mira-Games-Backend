using Domain.Modules.DataProcessing.Application.DTOs;
using Domain.Modules.DataProcessing.Interfaces.Repositories;
using Domain.Modules.DataProcessing.Models;
using Infrastructure.Shared.Database.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Modules.DataProcessing.Repositories
{
    /// <summary>
    /// Реализация репозитория для работы с данными игр
    /// Обрабатывает чтение из JSON файла и запись в базу данных
    /// </summary>
    public class GameDataDBRepository : IGameDataDBRepository
    {
        private readonly AppDBContext _appDBContext;
        private readonly string _gameInfoFilePath;
        private readonly ILogger<GameDataDBRepository> _logger;

        /// <summary>
        /// Инициализирует новый экземпляр репозитория
        /// </summary>
        /// <param name="appDBContext">Контекст базы данных Entity Framework</param>
        public GameDataDBRepository(AppDBContext appDBContext, ILogger<GameDataDBRepository> logger)
        {
            _appDBContext = appDBContext;
            _logger = logger;
            _gameInfoFilePath = "gameinfo.json"; // Файл для выгрузки данных об играх
        }

        /// <summary>
        /// Загружает данные игр из JSON файла
        /// </summary>
        /// <returns>Десериализованный список DTO или null если файл не найден</returns>
        public async Task<List<GameDataDBDto>?> LoadGameData()
        {
            _logger.LogInformation("⌚ Десериализуем JSON");
            if (File.Exists(_gameInfoFilePath))
            {
                using (FileStream openStream = File.OpenRead(_gameInfoFilePath))
                {
                    _logger.LogInformation("✅ JSON успешно десериализован");
                    return await JsonSerializer.DeserializeAsync<List<GameDataDBDto>>(openStream);
                }              
            }

            else return null;
        }

        /// <summary>
        /// Выполняет пакетное добавление и обновление данных игр в базе данных
        /// Для каждой игры проверяет существование по SteamId и выполняет соответствующую операцию
        /// </summary>
        /// <param name="gameDataDBOptions">Список DTO с данными игр для обработки</param>
        /// <exception cref="ArgumentException">Выбрасывается если передан null список</exception>
        public async Task AddAndUpdateGamesData(List<GameDataDBDto>? gameDataDBOptions)
        {
            if (gameDataDBOptions != null)
            {
                // Получаем ID всех игр из JSON для синхронизации и отсекаем дубликаты
                var jsonGameIds = gameDataDBOptions.Select(g => g.SteamId).ToHashSet();

                // Находим игры в БД, которых нет в JSON
                List<Game> gamesToDelete = await _appDBContext.Games
                    .Where(g => !jsonGameIds.Contains(g.SteamId))
                    .ToListAsync();

                // Удаляем игры, которых нет в JSON
                if (gamesToDelete.Any())
                {
                    _appDBContext.Games.RemoveRange(gamesToDelete);
                }

                _logger.LogInformation("🚀 Начинаем обработку данных");
                foreach (GameDataDBDto options in gameDataDBOptions)
                {
                    // Проверяем существование игры в базе данных
                    Game? existingGame = await _appDBContext.Games.FindAsync(options.SteamId);

                    if (existingGame == null) AddGamesData(options); // Добавляем новую игру
                    else if (existingGame != null) UpdateGamesData(options, existingGame); // Обновляем существующую
                }

                // Сохраняем все изменения одним запросом к базе данных
                await _appDBContext.SaveChangesAsync();
            }
            else throw new ArgumentException("Список не может быть null", nameof(gameDataDBOptions));
        }

        /// <summary>
        /// Создает и добавляет новую сущность игры в контекст базы данных
        /// </summary>
        /// <param name="options">DTO с данными для создания игры</param>
        private void AddGamesData(GameDataDBDto options)
        {
            _logger.LogInformation($"💾 Игра с Названием: {options.Title} и ID: {options.SteamId} - новая, сохраняем в базу данных");
            Game game = new Game(options.SteamId, options.Title, options.ReleaseDate, options.Genres, options.StoreURL, options.ImageURL, options.ShortDescription, options.SupportedPlatforms);
            _appDBContext.Games.Add(game);
        }

        /// <summary>
        /// Обновляет данные существующей игры в контексте базы данных
        /// </summary>
        /// <param name="options">DTO с новыми данными игры</param>
        /// <param name="existingGame">Существующая сущность игры для обновления</param>
        private void UpdateGamesData(GameDataDBDto options, Game existingGame)
        {
            _logger.LogInformation($"🔄 Игра с Названием: {options.Title} и ID: {options.SteamId} - уже существует, обновляем старые данные");
            // Если SteamId изменился в API - обновляем!
            if (existingGame.SteamId != options.SteamId)
            {
                existingGame.SteamId = options.SteamId;
            }

            existingGame.Title = options.Title;
            existingGame.ReleaseDate = options.ReleaseDate;
            existingGame.Genres = options.Genres;
            existingGame.StoreURL = options.StoreURL;
            existingGame.ImageURL = options.ImageURL;
            existingGame.ShortDescription = options.ShortDescription;
            existingGame.SupportedPlatforms = options.SupportedPlatforms;
        }
    }
}
