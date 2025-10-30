using Infrastructure.Shared.Database.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Shared.Extensions
{
    /// <summary>
    /// Расширения для настройки хоста приложения
    /// </summary>
    public static class HostExtensions
    {
        /// <summary>
        /// Выполняет миграции базы данных при запуске приложения
        /// </summary>
        public static IHost MigrateDatabase(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                // Миграции для PostgreSQL
                var postgresContext = services.GetRequiredService<AppDBContext>();
                postgresContext.Database.Migrate();

                // Инициализация ClickHouse
                var clickHouseInitializer = services.GetRequiredService<ClickHouseInitialization>();
                clickHouseInitializer.InitializeDatabaseAsync().GetAwaiter().GetResult();
            }
            return host;
        }
    }
}
