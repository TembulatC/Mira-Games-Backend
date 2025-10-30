using Domain.Modules.DataProcessing.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Shared.Database.TableConfigurations
{
    // Конфигурация таблицы Game в базе данных
    public class GameConfiguration : IEntityTypeConfiguration<Game>
    {
        public void Configure(EntityTypeBuilder<Game> builder)
        {
            // Настраивает таблицу Game и устанавливает SteamId как первичный ключ
            builder.HasKey(game => game.SteamId);
        }
    }
}
