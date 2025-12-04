using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Modules.Orchestrator.ScheduledUseCases
{
    /// <summary>
    /// Фоновая служба для выполнения запланированного обновления статистики игр
    /// Выполняет последовательный конвейер обработки данных с заданными интервалами
    /// </summary>
    public class GameUpdateStatisticsService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<GameUpdateStatisticsService> _logger;

        public GameUpdateStatisticsService(IServiceProvider serviceProvider, ILogger<GameUpdateStatisticsService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger; // Решил в случае с фоновыми процессами добавить логгирование
        }

        /// <summary>
        /// Основной метод выполнения фоновой службы
        /// Реализует бесконечный цикл с последовательным выполнением этапов обработки данных
        /// </summary>
        /// <param name="stoppingToken">Токен отмены для graceful shutdown</param>
        /// <returns>Task, представляющий асинхронную операцию</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Запуск последовательного конвейера данных");

                    using var scope = _serviceProvider.CreateScope();
                    var useCase = scope.ServiceProvider.GetRequiredService<GameUpdateStatisticsUseCase>();

                    // Этап 1: JSON (ждем 10 минут с момента последнего запуска)
                    _logger.LogInformation("Этап 1: Ожидание 10 минут для обновления JSON...");
                    await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);

                    _logger.LogInformation("Этап 1: Запуск обновления JSON");
                    await useCase.ScheduledGetGamesId();
                    _logger.LogInformation("Этап 1: Обновление JSON завершено");

                    // Этап 2: База данных (ждем 10 минут после этапа JSON)
                    _logger.LogInformation("Этап 2: Ожидание 10 минут для сохранения в базу данных...");
                    await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);

                    _logger.LogInformation("Этап 2: Запуск сохранения в базу данных");
                    await useCase.ScheduledAddAndUpdateGamesData();
                    _logger.LogInformation("Этап 2: Сохранение в базу данных завершено");

                    // Этап 3: ClickHouse (ждем 10 минут после этапа БД)
                    _logger.LogInformation("Этап 3: Ожидание 10 минут для экспорта в ClickHouse...");
                    await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);

                    _logger.LogInformation("Этап 3: Запуск экспорта в ClickHouse");
                    await useCase.ScheduledAddChangeDynamic();
                    _logger.LogInformation("Этап 3: Экспорт в ClickHouse завершен");

                    _logger.LogInformation("Последовательный конвейер данных завершен. Следующий цикл через 2 часа");
                    await Task.Delay(TimeSpan.FromHours(2), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogWarning("Конвейер данных был отменен");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, "⛔ Ошибка в последовательном конвейере данных, через 5 минут произойдет перезапуск конвейера");

                    // Ждем перед повторной попыткой при ошибке
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }
        }
    }
}
