using Domain.Modules.DataProcessing.Models;
using Infrastructure.Shared.Database.TableConfigurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Shared.Database.DBContext
{
    // Настройка EntityFramework
    public class AppDBContext(DbContextOptions<AppDBContext> options) : DbContext(options)
    {
        public DbSet<Game> Games { get; set; } // Создание таблицы Games

        // Добавление конфигураций таблицам
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new GameConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
