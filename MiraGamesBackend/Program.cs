using Microsoft.OpenApi.Models;

namespace MiraGamesBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ���������� ��������� ������������ � �������������
            builder.Services.AddControllersWithViews();

            // ����������� �������� ��� Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "API �������� ������� Mira Games",
                    Version = "v1",
                    Description = "Backend-������, ������� ��������, ���������� � ������������� ������ � ������� ������� ��� �� ������ �� Steam"
                });

                options.EnableAnnotations();
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSwagger(); // ���������� Swagger
            app.UseSwaggerUI(); // ���������� ����������������� ���������� Swagger

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.Run();
        }
    }
}
