using Infrastructure.Shared.Database.DBContext;
using Infrastructure.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ClickHouse.Client.ADO;
using Domain.Modules.SteamIntegration.Application.PipeLines;
using Domain.Modules.Orchestrator.UseCases;
using Infrastructure.Modules.SteamIntegration.Repositories;
using Domain.Modules.SteamIntegration.Application.Services;
using Infrastructure.Modules.DataProcessing.Repositories;
using Domain.Modules.DataProcessing.Application.Services;
using Domain.Modules.DataProcessing.Interfaces.Services;
using Domain.Modules.SteamIntegration.Interfaces.Services;
using Domain.Modules.DataProcessing.Interfaces.Repositories;
using Domain.Modules.SteamIntegration.Interfaces.Repositories;
using Domain.Modules.SteamIntegration.Interfaces.PipeLines;
using MiraGamesBackend.Utilities;
using Domain.Modules.ClickHouse.Interfaces.Repositories;
using Infrastructure.Modules.ClickHouse.Repositories;
using Domain.Modules.ClickHouse.Interfaces.Services;
using Domain.Modules.ClickHouse.Application.Services;
using Domain.Modules.Orchestrator.ScheduledUseCases;

namespace MiraGamesBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Добавление поддержки контроллеров и представлений
            builder.Services.AddControllersWithViews();

            // Подключение сервисов для Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "API Тестового задания Mira Games",
                    Version = "v1",
                    Description = "Backend-сервис, который собирает, агрегирует и предоставляет данные о будущих релизах игр на следующий из Steam"
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
            builder.Services.AddSingleton(provider =>
                new ClickHouseInitialization(
                builder.Configuration.GetConnectionString("ClickHouseConnection")));

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
    }
}
