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
                    Description = "Backend-сервис, который собирает, агрегирует и предоставляет данные о будущих релизах игр на ноябрь из Steam"
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
            builder.Services.AddScoped(provider =>
            {
                var connectionStringClickHouse = builder.Configuration.GetConnectionString("ClickHouseConnection");
                return new ClickHouseConnection(connectionStringClickHouse);
            });

            // Добавление HttpClient для отправки HTTP запросов в SteamAPI
            builder.Services.AddHttpClient();


            // Регистрируем репозитории модуля DataProcessing
            builder.Services.AddScoped<IGameDataDBRepository, GameDataDBRepository>();
            builder.Services.AddScoped<IGameCalendarDBRepository, GameCalendarDBRepository>();

            // Регистрируем Piplines и Services модуля DataProcessing
            builder.Services.AddScoped<IGameDataDBService, GameDataDBService>();
            builder.Services.AddScoped<IGameCalendarDBService, GameCalendarDBService>();


            // Регистрируем репозитории модуля SteamIntegration
            builder.Services.AddScoped<ISteamParseRepository, SteamParseIDRepository>();
            builder.Services.AddScoped<ISteamAPIRepository, SteamAPIRepository>();

            // Регистрируем Piplines и Services модуля SteamIntegration
            builder.Services.AddScoped<IGetSteamParsePipeLine, GetSteamParsePipeLine>();
            builder.Services.AddScoped<IGetSteamAPIService, GetSteamAPIService>();


            // Регистрируем UseCases модуля Orchestrator
            builder.Services.AddScoped<GetGamesDataUseCase>();
            builder.Services.AddScoped<GameDataDBUseCase>();
            builder.Services.AddScoped<GameCalendarUseCase>();

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

            app.MigrateDatabase();

            app.Run();
        }
    }
}
