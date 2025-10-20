using Infrastructure.Shared.Database.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

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

            app.Run();
        }
    }
}
