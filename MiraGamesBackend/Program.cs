using Infrastructure.Shared.Database.DBContext;
using Infrastructure.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ClickHouse.Client.ADO;
using Domain.Modules.SteamIntegration.Interfaces;
using Domain.Modules.SteamIntegration.Application.PipeLines;
using Domain.Modules.Orchestrator.UseCases;
using Infrastructure.Modules.SteamIntegration.Repositories;
using Domain.Modules.SteamIntegration.Application.Services;

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

            // Добавьте отдельный HttpClient для FlareSolverr с увеличенными таймаутами
            builder.Services.AddHttpClient("FlareSolverr", client =>
            {
                client.Timeout = TimeSpan.FromMinutes(5);
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
            });

            // Регистрируем репозитории
            builder.Services.AddScoped<ISteamParseRepository, SteamParseIDRepository>();
            builder.Services.AddScoped<ISteamAPIRepository, SteamAPIRepository>();

            // Регистрируем Piplines и Services
            builder.Services.AddScoped<IGetGamesIDPipeLine, GetGamesIDPipeLine>();
            builder.Services.AddScoped<IGetGamesDataService, GetGamesDataService>();

            // Регистрируем UseCases
            builder.Services.AddScoped<GetGamesDataUseCase>();

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
