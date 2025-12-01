using Domain.Modules.ClickHouse.Application.Services;
using Domain.Modules.ClickHouse.Interfaces.Repositories;
using Domain.Modules.ClickHouse.Interfaces.Services;
using Domain.Modules.DataProcessing.Application.Services;
using Domain.Modules.DataProcessing.Interfaces.Repositories;
using Domain.Modules.DataProcessing.Interfaces.Services;
using Domain.Modules.Orchestrator.ScheduledUseCases;
using Domain.Modules.Orchestrator.UseCases;
using Domain.Modules.SteamIntegration.Application.PipeLines;
using Domain.Modules.SteamIntegration.Application.Services;
using Domain.Modules.SteamIntegration.Interfaces.PipeLines;
using Domain.Modules.SteamIntegration.Interfaces.Repositories;
using Domain.Modules.SteamIntegration.Interfaces.Services;
using Infrastructure.Modules.ClickHouse.Repositories;
using Infrastructure.Modules.DataProcessing.Repositories;
using Infrastructure.Modules.SteamIntegration.Repositories;
using Infrastructure.Shared.Database.DBContext;
using Infrastructure.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MiraGamesBackend.Utilities;
using Serilog;
using Serilog.Events;

namespace MiraGamesBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Bootstrap Logger - для логирования до настройки DI
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {SourceContext} | {Message:lj}{NewLine}Application: {Application}{NewLine}Environment: {Environment}{NewLine}{Exception}")
                .CreateBootstrapLogger();

            try
            {
                Log.Information("Запуск Steam Analytics Backend...");

                var builder = WebApplication.CreateBuilder(args);

                builder.Host.UseSerilog((context, services, configuration) =>
                {
                    configuration
                        .MinimumLevel.Information()

                        // Фильтрация системных логов - показывать только WARNING и выше
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                        .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                        .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                        .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                        .MinimumLevel.Override("System", LogEventLevel.Warning)
                        .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Information)

                        // Обогащение
                        .Enrich.WithProperty("Application", "Steam Analytics Backend")
                        .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                        .Enrich.FromLogContext()

                        // Вывод в консоль
                        .WriteTo.Logger(lc => lc
                            .Filter.ByIncludingOnly(e => e.Level < LogEventLevel.Warning) // INF только
                            .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] {SourceContext:l} | {Message:lj}{NewLine}{Exception}"))

                        .WriteTo.Logger(lc => lc
                            .Filter.ByIncludingOnly(e => e.Level >= LogEventLevel.Warning) // WRN, ERR, FATAL
                            .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {SourceContext} | {Message:lj}{NewLine}Application: {Application}{NewLine}Environment: {Environment}{NewLine}{Exception}"))

                        // Вывод в файл
                        .WriteTo.File(
                            "Logs/steam-analytics-.log",
                            rollingInterval: RollingInterval.Day,
                            retainedFileCountLimit: 7,
                            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {SourceContext} | {Message:lj}{NewLine}Application: {Application}{NewLine}Environment: {Environment}{NewLine}{Exception}");
                });

                // Добавление поддержки контроллеров и представлений
                builder.Services.AddControllersWithViews();

                // Подключение сервисов для Swagger
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "API Steam Analytics Backend",
                        Version = "v1",
                        Description = "Backend-сервис, который собирает, агрегирует и предоставляет данные о будущих релизах игр на следующий месяц из Steam"
                    });

                    options.EnableAnnotations();

                    // Для отображения Display атрибутов
                    options.ParameterFilter<GameGenreParameterFilter>();
                    options.ParameterFilter<GameSupportPlatformsParameterFilter>();
                });

                // Добавляем AppDBContext и подключаем БД
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                builder.Services.AddDbContext<AppDBContext>(options => options.UseNpgsql(connectionString));

                // Подключаемся к ClickHouse
                builder.Services.AddSingleton<ClickHouseInitialization>(provider =>
                {
                    var connectionString = builder.Configuration.GetConnectionString("ClickHouseConnection");
                    var logger = provider.GetRequiredService<ILogger<ClickHouseInitialization>>();
                    return new ClickHouseInitialization(connectionString, logger);
                });

                // Добавление HttpClient для отправки HTTP запросов в SteamAPI
                builder.Services.AddHttpClient();


                // Регистрируем репозитории модуля DataProcessing
                builder.Services.AddScoped<IGameDataDBRepository, GameDataDBRepository>();
                builder.Services.AddScoped<IGameCalendarDBRepository, GameCalendarDBRepository>();
                builder.Services.AddScoped<IGameStatisticRepository, GameStatisticRepository>();

                // Регистрируем Piplines и Services модуля DataProcessing
                builder.Services.AddScoped<IGameDataDBService, GameDataDBService>();
                builder.Services.AddScoped<IGameCalendarDBService, GameCalendarDBService>();
                builder.Services.AddScoped<IGameStatisticService, GameStatisticService>();


                // Регистрируем репозитории модуля SteamIntegration
                builder.Services.AddScoped<ISteamParseRepository, SteamParseIDRepository>();
                builder.Services.AddScoped<ISteamAPIRepository, SteamAPIRepository>();

                // Регистрируем Piplines и Services модуля SteamIntegration
                builder.Services.AddScoped<IGetSteamParsePipeLine, GetSteamParsePipeLine>();
                builder.Services.AddScoped<IGetSteamAPIService, GetSteamAPIService>();


                // Регистрируем репозитории модуля ClickHouse
                builder.Services.AddScoped<IChangeDynamicsRepository, ChangeDynamicsRepository>();

                // Регистрируем репозитории Piplines и Services модуля ClickHouse
                builder.Services.AddScoped<IChangeDynamicsService, ChangeDynamicsService>();


                // Регистрируем UseCases модуля Orchestrator
                builder.Services.AddScoped<GetGamesDataUseCase>();
                builder.Services.AddScoped<GameDataDBUseCase>();
                builder.Services.AddScoped<GameCalendarUseCase>();
                builder.Services.AddScoped<GameStatisticUseCase>();
                builder.Services.AddScoped<GameUpdateStatisticsUseCase>();

                builder.Services.AddHostedService<GameUpdateStatisticsService>();

                var app = builder.Build();

                Log.Information("Steam Analytics Backend успешно запущен!");

                // Configure the HTTP request pipeline.
                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Error");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }

                app.UseSwagger(); // Добавление Swagger
                app.UseSwaggerUI(); // Добавление пользовательского интерфейса Swagger

                app.UseHttpsRedirection();
                app.UseStaticFiles();

                app.UseRouting();

                app.UseAuthorization();

                app.MapControllers();

                // Автоматическое выполнение миграций при запуске
                app.MigrateDatabase();

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Не удалось запустить Steam Analytics Backend...");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
