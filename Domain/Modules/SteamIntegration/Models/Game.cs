using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Modules.SteamIntegration.Models
{
    // Эта модель будет использоваться в Entity Framework Core для хранения актуальных данных об играх в PostgreSQL
    public class Game
    {   
        public Game() { }

        public Game(int _SteamId, string _Title, DateTime _ReleaseDate, List<string> _Genres, int _WishlistsCount, string _StoreURL, string _ImageURL, string _ShortDescription, List<string> _SupportedPlatforms)
        {
            SteamId = _SteamId;
            Title = _Title;
            ReleaseDate = _ReleaseDate;
            Genres = _Genres;
            WishlistsCount = _WishlistsCount;
            StoreURL = _StoreURL;
            ImageURL = _ImageURL;
            ShortDescription = _ShortDescription;
            SupportedPlatforms = _SupportedPlatforms;
        }

        public int SteamId { get; set; } // Уникальный идентификатор игры в Steam

        public string Title { get; set; } // Название игры

        public DateTime ReleaseDate { get; set; } // Дата релиза игры

        /*
         Решил выбрать только жанры, потому что тегов нет в SteamAPI (Либо я просто их не нашел), их можно получить только при парсинге HTML страницы игры
         Я подумал что сосредоточусь только на взаимодействии со SteamAPI, потому что как мне кажется для тестового хватит информации и статистики по жанрам
         Если данная идея вас не устроила можете засчитать это как замечание
        */
        public List<string> Genres { get; set; }

        public int WishlistsCount { get; set; } // Решил выбрать именно количество wishlists, так как это хорошо отражает именно коммерческий интерес к игре

        public string StoreURL { get; set; } // Ссылка на страницу игры в магазине

        public string ImageURL { get; set; } // Ссылка на официальный постер игры

        public string ShortDescription {  get; set; } // Короткое описание игры

        public List<string> SupportedPlatforms { get; set; } // Поддерживаемые платформы
    }
}
