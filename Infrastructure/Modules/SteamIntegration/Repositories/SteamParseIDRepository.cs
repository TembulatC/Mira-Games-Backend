using Domain.Modules.SteamIntegration.Application.DTOs;
using Domain.Modules.SteamIntegration.Application.DTOs.Responses;
using Domain.Modules.SteamIntegration.Interfaces.Repositories;
using HtmlAgilityPack;
using System.Globalization;
using System.Text.Json;

namespace Infrastructure.Modules.SteamIntegration.Repositories
{
    /// <summary>
    /// Репозиторий для парсинга Steam страниц с целью получения ID игр, выходящих в следующем месяце
    /// Реализует адаптивный алгоритм поиска, который отслеживает смещение релизов при обновлении каталога
    /// </summary>
    public class SteamParseIDRepository : ISteamParseRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _stateFilePath;

        /// <summary>
        /// Инициализирует новый экземпляр репозитория для парсинга Steam
        /// </summary>
        /// <param name="httpClient">HTTP клиент для выполнения запросов к Steam Store</param>
        public SteamParseIDRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;

            // Устанавливаем User-Agent для имитации браузера и избежания блокировки
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36");         
            _stateFilePath = "data-state.json"; // Файл для сохранения состояния парсера между запусками
        }

        /// <summary>
        /// Загружает состояние парсера из JSON файла
        /// Состояние содержит информацию о последней обработанной странице для оптимизации последующих запусков
        /// </summary>
        /// <returns>Состояние парсера или null если файл не существует</returns>
        public async Task<SteamParseResponse?> LoadState()
        {
            if (File.Exists(_stateFilePath))
            {
                using (FileStream openStream = File.OpenRead(_stateFilePath))
                {
                    return await JsonSerializer.DeserializeAsync<SteamParseResponse>(openStream);
                }             
            }

            else return null;
        }

        /// <summary>
        /// Сохраняет текущее состояние парсера в JSON файл
        /// Позволяет продолжить с последней обработанной страницы при следующем запуске
        /// </summary>
        /// <param name="saveStateResponse">Состояние парсера для сохранения</param>
        public async Task SaveState(SteamParseResponse saveStateResponse)
        {
            var json = JsonSerializer.Serialize(saveStateResponse, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            await File.WriteAllTextAsync(_stateFilePath, json);
        }

        /// <summary>
        /// Парсит Steam страницы для получения ID игр, выходящих в следующем месяце
        /// Использует адаптивную логику: начинает поиск на 2 страницы раньше последней известной,
        /// чтобы отслеживать смещение релизов при обновлении каталога. Алгоритм продолжает поиск
        /// до обнаружения релизов на другой месяц, что гарантирует сбор всех игр именно на следующий месяц.
        /// </summary>
        /// <param name="parseOptions">Настройки парсинга, включая стартовую страницу</param>
        /// <returns>Список ID игр с релизами</returns>
        public async Task<List<int>> ParseSteamGamesId(SteamParseDto parseOptions)
        {
            List<int> listID = new List<int>(); 
            
            int page = parseOptions.startPage - 2; // Начинаем поиск на 2 страницы раньше последней известной для отслеживания смещения
            int? findPage = null;
            bool foundMonth = false;
            
            DateTime today = DateTime.Today; // Формируем текущую дату
            CultureInfo englishCulture = new CultureInfo("en-US"); // Переводим название месяцев на английский

            Console.WriteLine($"🔍 Начинаем поиск с страницы: {page}");

            while (foundMonth == false) // Основной цикл парсинга: проходит по страницам пока не найдет релизы на другой месяц
            {
                string url = $"https://store.steampowered.com/search/?category1=998&filter=comingsoon&ndl=1&page={page}&count=100";
                string html = await _httpClient.GetStringAsync(url);

                await Task.Delay(1000); // Задержка для соблюдения лимитов запросов и избежания блокировки

                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                HtmlNodeCollection gameNodes = htmlDoc.DocumentNode.SelectNodes("//div[@id='search_resultsRows']/a"); // Ищем блок с результатами поиска

                if (gameNodes == null) // Если на странице нет игр, переходим к следующей
                {
                    page++;
                    continue;
                }

                foreach (HtmlNode node in gameNodes) // Обрабатываем каждую найденную игру на текущей странице
                {
                    string releaseDate = ExtractIdRerelaseDate(node);

                    if (releaseDate == string.Empty) continue;  // Пропускаем игры без указанной даты релиза

                    // Пропускаем релизы текущего месяца
                    if ((releaseDate.Contains(today.ToString("MMM", englishCulture)) || releaseDate.Contains(today.ToString("MMMM", englishCulture))) 
                        && releaseDate.Contains(today.ToString("yyyy", englishCulture)))
                    {
                        continue;
                    }

                    // Нашли релизы следующего месяца - добавляем ID игры
                    else if ((releaseDate.Contains(today.AddMonths(1).ToString("MMM", englishCulture)) || releaseDate.Contains(today.AddMonths(1).ToString("MMMM", englishCulture))) 
                        && releaseDate.Contains(today.AddMonths(1).ToString("yyyy", englishCulture)))
                    {
                        int? appId = ExtractId(node);

                        if (appId.HasValue)
                        {
                            // Сообщаем пайплайну о текущей странице для возможного обновления состояния
                            // Если мы нашли игры раньше ожидаемой страницы, пайплайн сохранит это смещение
                            if (findPage == null)
                            {
                                findPage = page;
                                Console.WriteLine($"🎯 Нашли игры следующего месяца на странице: {findPage}");
                            }
                            
                            listID.Add(appId.Value);
                        }
                        else continue;
                    }

                    // Нашли игры идущие после следующего месяца - отмечаем для завершения после обработки текущей страницы
                    else if ((releaseDate.Contains(today.AddMonths(2).ToString("MMM", englishCulture)) || releaseDate.Contains(today.AddMonths(2).ToString("MMMM", englishCulture))) 
                        && releaseDate.Contains(today.AddMonths(2).ToString("yyyy", englishCulture)))
                    {
                        foundMonth = true;
                        // НЕ завершаем сразу, обрабатываем всю страницу
                    }
                }

                page++; // Переходим к следующей странице          

            }

            if (findPage.HasValue) // Обновляем стартовую страницу для следующего запуска
            {
                Console.WriteLine($"🔄 Устанавливаем стартовую страницу: {findPage}");
                parseOptions.startPage = findPage.Value;
            }
            else parseOptions.startPage = 1;

            Console.WriteLine($"✅ Найдено игр: {listID.Count}");
            return listID;
        }

        #region Help Methods

        /// <summary>
        /// Извлекает ID игры из HTML ноды карточки игры
        /// Использует data-атрибут data-ds-appid, содержащий уникальный идентификатор игры в Steam
        /// </summary>
        /// <param name="htmlNode">HTML нода карточки игры</param>
        /// <returns>ID игры или null если не удалось извлечь</returns>
        private int? ExtractId(HtmlNode htmlNode)
        {
            string appIdStr = htmlNode.GetAttributeValue("data-ds-appid", "");

            if (int.TryParse(appIdStr, out int appId)) return appId;
            else return null;
        }

        /// <summary>
        /// Извлекает дату релиза из HTML ноды карточки игры
        /// Ищет элемент с классом search_released, содержащий текстовое представление даты выхода
        /// </summary>
        /// <param name="htmlNode">HTML нода карточки игры</param>
        /// <returns>Текст даты релиза или пустую строку если не найден</returns>
        private string ExtractIdRerelaseDate(HtmlNode htmlNode)
        {         
            HtmlNode dateNode = htmlNode.SelectSingleNode(".//div[contains(@class, 'search_released')]"); // Ищем дату релиза по классу search_released

            if (dateNode != null)
            {
                string releaseDate = dateNode.InnerText.Trim();               

                if (!string.IsNullOrEmpty(releaseDate)) return releaseDate;               

                else return string.Empty;
                
            }

            else return string.Empty;
        }

        #endregion
    }
}
