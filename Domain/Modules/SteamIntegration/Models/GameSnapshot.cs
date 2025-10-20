using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Modules.SteamIntegration.Models
{
    // Эта модель предназначена для хранения исторических данных в ClickHouse.
    public class GameSnapshot
    {
        public GameSnapshot() { }

        public GameSnapshot(int _SteamId, string _Title, int _WishlistsCount, List<string> _Genres)
        {
            SteamId = _SteamId;
            Title = _Title;
            CollectionDate = DateTime.Now;
            WishlistsCount = _WishlistsCount;
            Genres = _Genres;
        }

        public int SteamId { get; set; } // Уникальный идентификатор игры в Steam

        public string Title { get; set; } // Название игры

        public DateTime CollectionDate { get; set; } // Дата и времмя сбора данных

        public int WishlistsCount { get; set; } // Решил выбрать именно количество wishlists, так как это хорошо отражает именно коммерческий интерес к игре

        /*
         Решил выбрать только жанры, потому что тегов нет в SteamAPI (Либо я просто их не нашел), их можно получить только при парсинге HTML страницы игры
         Я подумал что сосредоточусь только на взаимодействии со SteamAPI, потому что как мне кажется для тестового хватит информации и статистики по жанрам
         Если данная идея вас не устроила можете засчитать это как замечание
        */
        public List<string> Genres { get; set; }
    }
}
