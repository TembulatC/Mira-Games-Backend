using Domain.Modules.SteamIntegration.Application.DTOs;
using Domain.Modules.SteamIntegration.Application.DTOs.Responses;
using Domain.Modules.SteamIntegration.Interfaces.PipeLines;
using Domain.Modules.SteamIntegration.Interfaces.Repositories;

namespace Domain.Modules.SteamIntegration.Application.PipeLines
{
    /// <summary>
    /// Пайплайн для получения ID релизов игр на следующий месяц из Steam
    /// Управляет всем процессом: загрузка состояния → парсинг → сохранение состояния
    /// Запускается вручную разработчиком когда требуются свежие данные
    /// </summary>
    public class GetSteamParsePipeLine : IGetSteamParsePipeLine
    {
        private readonly ISteamParseRepository _steamParseRepository;

        /// <summary>
        /// Инициализирует новый экземпляр пайплайна для получения ID игр
        /// </summary>
        /// <param name="steamParseRepository">Репозиторий для парсинга Steam страниц</param>
        public GetSteamParsePipeLine(ISteamParseRepository steamParseRepository)
        {
            _steamParseRepository = steamParseRepository;
        }

        /// <summary>
        /// Основной метод пайплайна для получения ID релизов игр на следующий месяц
        /// Выполняет полный цикл: загрузка состояния → парсинг → сохранение обновленного состояния
        /// 
        /// Алгоритм работы:
        /// 1. Загружает последнее известное состояние парсера (стартовая страница)
        /// 2. Если состояние не найдено, использует значения по умолчанию (стартовая страница: 1)
        /// 3. Запускает парсинг Steam страниц для поиска релизов на следующий месяц
        /// 4. Сохраняет обновленное состояние (включая возможное смещение стартовой страницы)
        /// 5. Возвращает список найденных ID игр
        /// 
        /// Особенности:
        /// - Парсер использует адаптивную логику: начинает поиск на 2 страницы раньше последней известной
        /// - Это позволяет автоматически обнаруживать смещение будующих релизов на более ранние страницы
        /// - При обнаружении смещения, новая стартовая страница сохраняется для следующего запуска
        /// - Метод запускается вручную разработчиком когда требуются актуальные данные
        /// </summary>
        /// <returns>Список ID игр, выходящих в следующем месяце. Пустой список если игры не найдены или произошла ошибка.</returns>
        public async Task<List<int>> GetGamesId()
        {
            // Шаг 1: Загрузка предыдущего состояния парсера
            SteamParseResponse? loadStateResponse = await _steamParseRepository.LoadState();

            SteamParseDto options;

            // Шаг 2: Инициализация настроек парсинга на основе сохраненного состояния
            if (loadStateResponse != null && loadStateResponse.SteamParser.StartPage >= 1)
            {
                options = new SteamParseDto
                {
                    startPage = loadStateResponse.SteamParser.StartPage
                };
            }
            else
            {
                options = new SteamParseDto
                {
                    startPage = 1
                };
            }

            // Шаг 3: Запуск парсинга Steam для получения ID игр
            List<int> appsId = await _steamParseRepository.ParseSteamGamesId(options);

            // Шаг 4: Сохранение обновленного состояния для следующего запуска
            SteamParseResponse? saveStateResponse = new SteamParseResponse
            {
                SteamParser = new SteamParser
                {
                    StartPage = options.startPage
                }
            };

            await _steamParseRepository.SaveState(saveStateResponse);

            return appsId;
        }
    }
}
