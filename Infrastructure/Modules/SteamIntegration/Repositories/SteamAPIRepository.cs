using Domain.Modules.SteamIntegration.Application.DTOs;
using Domain.Modules.SteamIntegration.Application.DTOs.Responses;
using Domain.Modules.SteamIntegration.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;

namespace Infrastructure.Modules.SteamIntegration.Repositories
{
    public class SteamAPIRepository : ISteamAPIRepository
    {
        private readonly HttpClient _httpClient;
        private readonly Random _random;
        private readonly string _gameInfoFilePath;
        private readonly ILogger<SteamAPIRepository> _logger;

        /// <summary>
        /// Конструктор репозитория для работы с Steam API
        /// </summary>
        /// <param name="httpClient">HTTP клиент для выполнения запросов</param>
        public SteamAPIRepository(HttpClient httpClient, ILogger<SteamAPIRepository> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _random = new Random();
            _gameInfoFilePath = "gameinfo.json"; // Файл для записи данных об играх
        }

        /// <summary>
        /// Получает данные об играх из Steam API по списку идентификаторов
        /// </summary>
        /// <param name="appId">Список Steam AppID для получения данных</param>
        /// <returns>Список объектов с данными об играх</returns>
        public async Task<List<SteamAPIResponse>> GetGamesData(List<int> appId)
        {
            List<SteamAPIResponse> gameData = new List<SteamAPIResponse>();

            // Проходим по всем переданным AppID
            _logger.LogInformation("🚀 Начинаем запись игр из SteamAPI в список");
            foreach (int steamId in appId) 
            {
                bool retry = false; // Флаг для повтора запроса в случае исключения

                while (retry != true)
                {
                    try
                    {
                        var response = await _httpClient.GetFromJsonAsync<Dictionary<string, SteamAPIResponse>>($"https://store.steampowered.com/api/appdetails?appids={steamId}"); // Выполняем запрос к Steam API для получения данных об игре
                        response.TryGetValue(steamId.ToString(), out SteamAPIResponse gameDataResponse); // Извлекаем данные игры из словаря ответа
                        gameData.Add(gameDataResponse); // Добавляем игру в список
                        _logger.LogInformation($"✅ Игра с ID: {steamId} и Названием: {gameDataResponse.data.name} добавлена");

                        int delayMs = _random.Next(2000, 5001); // Случайная задержка от 2 до 5 секунд
                        await Task.Delay(delayMs); // Добавляем задержку чтобы не превысить лимиты Steam API

                        retry = true; // Продолжаем

                    }
                    catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.BadGateway) // Если все таки превысили лимиты SteamAPI, то ждем 10 минут и повторяем
                    {
                        _logger.LogError(ex, "❌ Ошибка 502: Steam заблокировал обращение к API, ждем 10 минут и продолжаем парсинг");
                        await Task.Delay(600000);

                        retry = false; // Пробуем снова
                    }
                    catch (Exception ex) // Любая другая ошибка - полная остановка парсера
                    {               
                        _logger.LogCritical(ex, "⛔ Критическая ошибка: Парсер полностью останавливается!");
                        throw;
                    }
                }             
            }

            return gameData;
        }

        /// <summary>
        /// Сохраняет данные об играх в JSON файл
        /// Фильтрует только игры, выходящие в следующем месяце
        /// </summary>
        /// <param name="gameDataResponses">Список объектов с данными об играх для сохранения</param>
        public async Task SaveGamesData(List<SteamAPIResponse> gameDataResponses)
        {
            List<SteamAPIDto> allGamesData = new List<SteamAPIDto>();

            DateTime today = DateTime.Today; // Формируем текущую дату
            CultureInfo englishCulture = new CultureInfo("en-US"); // Переводим название месяцев на английский

            _logger.LogInformation($"🎛️ Фильтрация игр на {today.AddMonths(1):MMMM} {today.AddMonths(1):yyyy}");
            foreach (SteamAPIResponse gameData in gameDataResponses)
            {
                // Проверяем условия для сохранения игры:
                // 1. Успешный ответ от API
                // 2. Игра находится в статусе "coming_soon" (скоро выйдет)
                // 3. Дата релиза содержит название следующего месяца
                if (gameData.success == true 
                    && gameData?.data?.releaseDate?.coming_soon == true 
                    && (gameData?.data?.releaseDate?.date.Contains(today.AddMonths(1).ToString("MMM", englishCulture)) == true || gameData?.data?.releaseDate?.date.Contains(today.AddMonths(1).ToString("MMMM", englishCulture)) == true))
                {
                    SteamAPIDto options = new SteamAPIDto // Создаем объект с оптимизированными данными для сохранения
                    {
                        SteamId = gameData.data.appid,
                        Title = gameData.data.name,
                        ReleaseDate = DateOnly.Parse(gameData.data.releaseDate.date),
                        Genres = GetGenres(gameData),
                        StoreURL = $"https://store.steampowered.com/app/{gameData.data.appid}",
                        ImageURL = gameData.data.imageURL,
                        ShortDescription = gameData.data.shortDescription,
                        SupportedPlatforms = GetSupportedPlatforms(gameData),
                    };
                    
                    allGamesData.Add(options); // Добавляем игру в список для сохранения
                    _logger.LogInformation($"✅ Игра {gameData.data.name} добавлена для записи");
                }

                else continue; // Пропускаем игру если не прошла фильтр
            }

            if (allGamesData.Any()) // Сохраняем данные в JSON файл, если есть что сохранять
            {
                var json = JsonSerializer.Serialize(allGamesData, new JsonSerializerOptions
                {
                    WriteIndented = true // Форматируем JSON для читаемости
                });
                await File.WriteAllTextAsync(_gameInfoFilePath, json);
                _logger.LogInformation($"✅ Сохранено и записано {allGamesData.Count} игр");
            }
            else
            {
                _logger.LogInformation("⚠️ Нет данных для сохранения");
            }
        }

        #region Help Methods

        /// <summary>
        /// Извлекает список жанров из данных игры
        /// </summary>
        /// <param name="gameData">Объект с данными об игре</param>
        /// <returns>Список названий жанров</returns>
        private List<string> GetGenres(SteamAPIResponse gameData)
        {
            List<string> genresList = new List<string>();

            if (gameData.data.genres != null)
            {
                // Проходим по всем жанрам игры и добавляем их описания
                foreach (Genre genres in gameData.data.genres)
                {
                    genresList.Add(genres.description);
                }
            }
            else genresList = ["None"]; // Если список жанров отсутствует

            return genresList;
        }

        /// <summary>
        /// Извлекает список поддерживаемых платформ из данных игры
        /// Использует рефлексию для автоматического определения доступных платформ
        /// </summary>
        /// <param name="gameData">Объект с данными об игре</param>
        /// <returns>Список названий поддерживаемых платформ</returns>
        private List<string> GetSupportedPlatforms(SteamAPIResponse gameData)
        {
            List<string> supportedPlatformsList = new List<string>();

            // Получаем все свойства класса SupportedPlatforms с помощью рефлексии
            Type type = typeof(SupportedPlatforms);
            PropertyInfo[] properties = type.GetProperties();

            // Проверяем каждое свойство (платформу) на поддержку
            foreach (PropertyInfo property in properties)
            {          
                var value = property.GetValue(gameData.data.supportedPlatforms); // Получаем значение свойства true/false

                if (value is bool valueBool && valueBool == true) // Если платформа поддерживается, добавляем ее в список
                {                  
                    var name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(property.Name); // Преобразование названия (windows -> Windows)
                    supportedPlatformsList.Add(name);
                }
                else continue; // Пропускаем неподдерживаемые платформы
            }

            return supportedPlatformsList;
        }

        #endregion
    }
}