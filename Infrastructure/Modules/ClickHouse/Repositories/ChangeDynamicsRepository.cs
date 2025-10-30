using ClickHouse.Client.ADO;
using Domain.Modules.ClickHouse.Application.DTOs;
using Domain.Modules.ClickHouse.Interfaces.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Modules.ClickHouse.Repositories
{
    /// <summary>
    /// Репозиторий для работы с динамикой изменений в ClickHouse
    /// Обеспечивает запись статистических данных в таблицу game_snapshots
    /// </summary>
    public class ChangeDynamicsRepository : IChangeDynamicsRepository
    {
        private readonly string _connectionString;

        public ChangeDynamicsRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ClickHouseConnection");
        }

        /// <summary>
        /// Добавляет запись о динамике изменений в таблицу ClickHouse
        /// </summary>
        /// <param name="statistic">DTO объект со статистическими данными для сохранения</param>
        /// <returns>Task, представляющий асинхронную операцию вставки данных</returns>
        public async Task AddChangeDynamic(ClickHouseStatisticDto statistic)
        {
            using var connection = new ClickHouseConnection(_connectionString);
            await connection.OpenAsync();

            // SQL запрос для вставки данных в таблицу game_snapshots
            // Используется вложенная структура PopularGenres с массивами жанров и количества игр
            var insertQuery = @"
                INSERT INTO MiraGamesTest.game_snapshots 
                (Date, PopularGenres.Genre, PopularGenres.Games) 
                VALUES (@Date, @Genres, @Games)";

            using DbCommand command = connection.CreateCommand();
            command.CommandText = insertQuery;

            // Добавление параметра для Date
            var dateParam = command.CreateParameter();
            dateParam.ParameterName = "Date";
            dateParam.Value = statistic.date;
            command.Parameters.Add(dateParam);

            // Добавление параметра для Genres
            var genresParam = command.CreateParameter();
            genresParam.ParameterName = "Genres";
            genresParam.Value = statistic.popularGenres.Select(pg => pg.genre).ToArray();
            command.Parameters.Add(genresParam);

            // Добавление параметра для Games
            var gamesParam = command.CreateParameter();
            gamesParam.ParameterName = "Games";
            gamesParam.Value = statistic.popularGenres.Select(pg => pg.games).ToArray();
            command.Parameters.Add(gamesParam);

            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Получает записи о динамике изменений статистики игр за указанный период
        /// Извлекает снимки данных из ClickHouse для анализа трендов во времени
        /// </summary>
        /// <param name="date">Дата в формате "YYYY-MM" для фильтрации данных</param>
        /// <returns>Список DTO объектов с данными статистики за указанный период</returns>
        public async Task<List<ClickHouseStatisticDto>> GetChangeDynamic(string date)
        {
            using var connection = new ClickHouseConnection(_connectionString);
            await connection.OpenAsync();

            // SQL запрос для получения данных из таблицы game_snapshots
            // Используем LIKE для фильтрации по формату yyyy-MM
            var selectQuery = @"
                SELECT 
                    Date,
                    PopularGenres.Genre,
                    PopularGenres.Games
                FROM MiraGamesTest.game_snapshots 
                WHERE toString(Date) LIKE @DatePattern
                ORDER BY Date";

            // Выполнение запроса и получение DataReader для чтения результатов
            using DbCommand command = connection.CreateCommand();
            command.CommandText = selectQuery;

            // Добавление параметра для фильтрации по дате
            var dateParam = command.CreateParameter();
            dateParam.ParameterName = "DatePattern"; // Установка имени параметра (совпадает с именем в SQL запросе)
            dateParam.Value = $"{date}%"; // Добавляем % для LIKE поиска
            command.Parameters.Add(dateParam); // Добавление параметра в коллекцию параметров команды

            using var reader = await command.ExecuteReaderAsync();

            var result = new List<ClickHouseStatisticDto>();

            // Временный словарь для группировки данных по дате
            // Ключ - DateTime (дата), значение - DTO объект
            var tempData = new Dictionary<DateTime, ClickHouseStatisticDto>();

            // Цикл чтения каждой строки из результата запроса
            while (await reader.ReadAsync())
            {
                var recordDate = reader.GetDateTime("Date"); // Получение значения даты из колонки "Date"
                var genres = reader.GetFieldValue<string[]>("PopularGenres.Genre"); // Получение массива жанров из вложенной структуры PopularGenres.Genre
                var games = reader.GetFieldValue<int[]>("PopularGenres.Games"); // Получение массива количества игр из вложенной структуры PopularGenres.Games

                // Проверка, есть ли уже запись для этой даты в словаре
                // Если нет - создаем новую запись
                if (!tempData.ContainsKey(recordDate))
                {
                    tempData[recordDate] = new ClickHouseStatisticDto // Создание нового DTO объекта для текущей даты
                    {
                        date = recordDate,
                        popularGenres = new PopularGenres[genres.Length] // Инициализация массива PopularGenres с длиной равной количеству жанров
                    };
                }

                // Создаем массив PopularGenres для текущей даты
                var popularGenresArray = new PopularGenres[genres.Length];
                for (int i = 0; i < genres.Length; i++)
                {
                    popularGenresArray[i] = new PopularGenres // Создание объекта PopularGenres для каждой пары жанр-количество игр
                    {
                        genre = genres[i],
                        games = games[i]
                    };
                }

                tempData[recordDate].popularGenres = popularGenresArray; // Сохранение созданного массива в DTO объект для текущей даты
            }

            // Преобразование значений словаря в список
            // Values.ToList() возвращает все DTO объекты из словаря как List
            result = tempData.Values.ToList();
            return result;
        }
    }
}
