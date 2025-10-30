using ClickHouse.Client.ADO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Shared.Extensions
{
    /// <summary>
    /// Класс для инициализации базы данных ClickHouse
    /// </summary>
    public class ClickHouseInitialization
    {
        private readonly string _connectionString;

        public ClickHouseInitialization(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Асинхронно инициализирует базу данных и таблицы в ClickHouse
        /// Создает базу данных MiraGamesTest и таблицу game_snapshots если они не существуют
        /// Добавляет начальные данные с историей популярных жанров если таблица пустая
        /// </summary>
        /// <returns>Task, представляющий асинхронную операцию инициализации</returns>
        /// <exception cref="Exception">Выбрасывается при ошибках подключения или выполнения запросов</exception>
        public async Task InitializeDatabaseAsync()
        {
            try
            {
                using var connection = new ClickHouseConnection(_connectionString);
                await connection.OpenAsync();

                // Создаем базу данных если не существует
                var createDbQuery = @"CREATE DATABASE IF NOT EXISTS MiraGamesTest";
                using var dbCommand = connection.CreateCommand();
                dbCommand.CommandText = createDbQuery;
                await dbCommand.ExecuteNonQueryAsync();

                // Создаем таблицу если не существует
                var createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS MiraGamesTest.game_snapshots
                    (
                        Date DateTime,
                        PopularGenres Nested (
                            Genre String,
                            Games Int32
                        )
                    )
                    ENGINE = MergeTree()
                    PARTITION BY toYYYYMM(Date)
                    ORDER BY (Date)";

                using var tableCommand = connection.CreateCommand();
                tableCommand.CommandText = createTableQuery;
                await tableCommand.ExecuteNonQueryAsync();

                // Проверяем есть ли данные в таблице и добавляем начальные если пусто
                await InsertInitialDataIfEmptyAsync(connection);

                Console.WriteLine("ClickHouse database and table initialized successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing ClickHouse: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Вставляет начальные данные с историей популярных жанров если таблица пустая
        /// </summary>
        /// <param name="connection">Открытое подключение к ClickHouse</param>
        /// <returns>Task, представляющий асинхронную операцию вставки данных</returns>
        private async Task InsertInitialDataIfEmptyAsync(ClickHouseConnection connection)
        {
            // Проверяем есть ли данные в таблице
            var checkDataQuery = "SELECT count() FROM MiraGamesTest.game_snapshots";
            using var checkCommand = connection.CreateCommand();
            checkCommand.CommandText = checkDataQuery;
            var count = await checkCommand.ExecuteScalarAsync();

            if (Convert.ToInt32(count) == 0)
            {
                // Начальные данные с историей популярных жанров
                var initialData = new[]
                {
                    new {
                        Date = new DateTime(2025, 10, 30, 12, 45, 16),
                        Genres = new[] { "Indie", "Adventure", "Casual", "Action", "Simulation" },
                        Games = new[] { 740, 417, 408, 371, 248 }
                    },
                    new {
                        Date = new DateTime(2025, 10, 30, 12, 31, 16),
                        Genres = new[] { "Indie", "Adventure", "Casual", "Action", "Simulation" },
                        Games = new[] { 762, 425, 419, 379, 257 }
                    },
                    new {
                        Date = new DateTime(2025, 10, 30, 04, 43, 13),
                        Genres = new[] { "Indie", "Adventure", "Casual", "Action", "Simulation" },
                        Games = new[] { 752, 421, 412, 376, 253 }
                    },
                    new {
                        Date = new DateTime(2025, 10, 30, 02, 33, 57),
                        Genres = new[] { "Indie", "Adventure", "Casual", "Action", "Simulation" },
                        Games = new[] { 752, 421, 412, 376, 253 }
                    },
                    new {
                        Date = new DateTime(2025, 10, 29, 22, 51, 44),
                        Genres = new[] { "Indie", "Adventure", "Casual", "Action", "Simulation" },
                        Games = new[] { 680, 380, 373, 330, 231 }
                    },
                    new {
                        Date = new DateTime(2025, 10, 29, 22, 47, 05),
                        Genres = new[] { "Indie", "Adventure", "Casual", "Action", "Simulation" },
                        Games = new[] { 680, 380, 373, 330, 231 }
                    }
                };

                // Вставляем начальные данные
                foreach (var data in initialData)
                {
                    var insertQuery = @"
                        INSERT INTO MiraGamesTest.game_snapshots 
                        (Date, `PopularGenres.Genre`, `PopularGenres.Games`) 
                        VALUES (@Date, @Genres, @Games)";

                    using var insertCommand = connection.CreateCommand();
                    insertCommand.CommandText = insertQuery;

                    // Параметр даты
                    var dateParam = insertCommand.CreateParameter();
                    dateParam.ParameterName = "Date";
                    dateParam.Value = data.Date;
                    insertCommand.Parameters.Add(dateParam);

                    // Параметр жанров
                    var genresParam = insertCommand.CreateParameter();
                    genresParam.ParameterName = "Genres";
                    genresParam.Value = data.Genres;
                    insertCommand.Parameters.Add(genresParam);

                    // Параметр количества игр
                    var gamesParam = insertCommand.CreateParameter();
                    gamesParam.ParameterName = "Games";
                    gamesParam.Value = data.Games;
                    insertCommand.Parameters.Add(gamesParam);

                    await insertCommand.ExecuteNonQueryAsync();
                }

                Console.WriteLine($"Initial data inserted: {initialData.Length} historical records");
            }
            else
            {
                Console.WriteLine("Table already contains data, skipping initial data insertion");
            }
        }
    }
}