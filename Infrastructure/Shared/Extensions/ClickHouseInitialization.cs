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
        /// </summary>
        /// <returns>Task, представляющий асинхронную операцию инициализации</returns>
        /// <exception cref="Exception">Выбрасывается при ошибках подключения или выполнения запросов</exception>
        public async Task InitializeDatabaseAsync()
        {
            try
            {
                using var connection = new ClickHouseConnection(_connectionString);
                await connection.OpenAsync();

                var createDbQuery = @"CREATE DATABASE IF NOT EXISTS MiraGamesTest";
                using var dbCommand = connection.CreateCommand();
                dbCommand.CommandText = createDbQuery;
                await dbCommand.ExecuteNonQueryAsync();

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

                Console.WriteLine("ClickHouse database and table initialized successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing ClickHouse: {ex.Message}");
                throw;
            }
        }
    }
}
