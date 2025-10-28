using Domain.Modules.SteamIntegration.Application.DTOs.Options;
using Domain.Modules.SteamIntegration.Application.DTOs.Responses;
using Domain.Modules.SteamIntegration.Interfaces;
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

        /// <summary>
        /// Конструктор репозитория для работы с Steam API
        /// </summary>
        /// <param name="httpClient">HTTP клиент для выполнения запросов</param>
        public SteamAPIRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _random = new Random();
            _gameInfoFilePath = "gameinfo.json"; // Файл для записи данных об играх
        }

        /// <summary>
        /// Получает данные об играх из Steam API по списку идентификаторов
        /// </summary>
        /// <param name="appId">Список Steam AppID для получения данных</param>
        /// <returns>Список объектов с данными об играх</returns>
        public async Task<List<GameDataResponse>> GetGamesData(List<int> appId)
        {
            List<GameDataResponse> gameData = new List<GameDataResponse>();

            // Проходим по всем переданным AppID
            Console.WriteLine("🔥 Начинаем запись игр из SteamAPI в список");
            foreach (int steamId in appId) 
            {
                try
                {
                    var response = await _httpClient.GetFromJsonAsync<Dictionary<string, GameDataResponse>>($"https://store.steampowered.com/api/appdetails?appids={steamId}"); // Выполняем запрос к Steam API для получения данных об игре
                    response.TryGetValue(steamId.ToString(), out GameDataResponse gameDataResponse); // Извлекаем данные игры из словаря ответа
                    gameData.Add(gameDataResponse); // Добавляем игру в список
                    Console.WriteLine($"✅ Игра с ID: {steamId} и Названием: {gameDataResponse.data.name} добавлена");

                    int delayMs = _random.Next(2000, 5001); // Случайная задержка от 2 до 5 секунд
                    await Task.Delay(delayMs); // Добавляем задержку чтобы не превысить лимиты Steam API
                }
                catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.BadGateway) // Если все таки превысили лимиты SteamAPI, то ждем еще 10 секунд и повторяем
                {
                    // ✅ Повтор в отдельном try-catch
                    try
                    {
                        await Task.Delay(10001);

                        var response = await _httpClient.GetFromJsonAsync<Dictionary<string, GameDataResponse>>($"https://store.steampowered.com/api/appdetails?appids={steamId}"); // Выполняем запрос к Steam API для получения данных об игре
                        response.TryGetValue(steamId.ToString(), out GameDataResponse gameDataResponse); // Извлекаем данные игры из словаря ответа
                        gameData.Add(gameDataResponse); // Добавляем игру в список
                        Console.WriteLine($"✅ Игра с ID: {steamId} и Названием: {gameDataResponse.data.name} добавлена");
                    }
                    catch (Exception retryEx)
                    {
                        Console.WriteLine($"❌ Повторная попытка для AppID {steamId} также не удалась: {retryEx.Message}");
                    }
                }
            }

            return gameData;
        }

        /// <summary>
        /// Сохраняет данные об играх в JSON файл
        /// Фильтрует только игры, выходящие в ноябре
        /// </summary>
        /// <param name="gameDataResponses">Список объектов с данными об играх для сохранения</param>
        public async Task SaveGamesData(List<GameDataResponse> gameDataResponses)
        {
            List<GameDataOptions> allGamesData = new List<GameDataOptions>();

            Console.WriteLine("🔥 Фильтрация ноябрьских игр");
            foreach (GameDataResponse gameData in gameDataResponses)
            {
                // Проверяем условия для сохранения игры:
                // 1. Успешный ответ от API
                // 2. Игра находится в статусе "coming_soon" (скоро выйдет)
                // 3. Дата релиза содержит "Nov" или "November" (ноябрьский релиз)
                if (gameData.success == true 
                    && gameData?.data?.releaseDate?.coming_soon == true 
                    && (gameData?.data?.releaseDate?.date.Contains("Nov") == true || gameData?.data?.releaseDate?.date.Contains("November") == true))
                {
                    GameDataOptions options = new GameDataOptions // Создаем объект с оптимизированными данными для сохранения
                    {
                        SteamId = gameData.data.appid,
                        Title = gameData.data.name,
                        ReleaseDate = DateTime.Parse(gameData.data.releaseDate.date),
                        Genres = GetGenres(gameData),
                        StoreURL = $"https://store.steampowered.com/app/{gameData.data.appid}",
                        ImageURL = gameData.data.imageURL,
                        ShortDescription = gameData.data.shortDescription,
                        SupportedPlatforms = GetSupportedPlatforms(gameData),
                    };
                    
                    allGamesData.Add(options); // Добавляем игру в список для сохранения
                    Console.WriteLine($"✅ Игра {gameData.data.name} добавлена для записи");
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
                Console.WriteLine($"✅ Сохранено и записано {allGamesData.Count} игр");
            }
            else
            {
                Console.WriteLine("⚠️ Нет данных для сохранения");
            }
        }

        #region Help Methods

        /// <summary>
        /// Извлекает список жанров из данных игры
        /// </summary>
        /// <param name="gameData">Объект с данными об игре</param>
        /// <returns>Список названий жанров</returns>
        private List<string> GetGenres(GameDataResponse gameData)
        {
            List<string> genresList = new List<string>();

            // Проходим по всем жанрам игры и добавляем их описания
            foreach (Genre genres in gameData.data.genres)
            {
                genresList.Add(genres.description);
            }

            return genresList;
        }

        /// <summary>
        /// Извлекает список поддерживаемых платформ из данных игры
        /// Использует рефлексию для автоматического определения доступных платформ
        /// </summary>
        /// <param name="gameData">Объект с данными об игре</param>
        /// <returns>Список названий поддерживаемых платформ</returns>
        private List<string> GetSupportedPlatforms(GameDataResponse gameData)
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