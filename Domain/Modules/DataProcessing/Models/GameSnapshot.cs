namespace Domain.Modules.DataProcessing.Models
{
    // Эта модель предназначена для хранения исторических данных в ClickHouse.
    public class GameSnapshot
    {
        public GameSnapshot() { }

        public GameSnapshot(int _SteamId, string _Title, int _FollowersCount, List<string> _Genres)
        {
            SteamId = _SteamId;
            Title = _Title;
            CollectionDate = DateTime.Now;
            FollowersCount = _FollowersCount;
            Genres = _Genres;
        }

        public int SteamId { get; set; } // Уникальный идентификатор игры в Steam

        public string Title { get; set; } // Название игры

        public DateTime CollectionDate { get; set; } // Дата и времмя сбора данных

        public int FollowersCount { get; set; } // Решил выбрать именно количество followers, так как многие API не предоставляют данные о вишлистах(а иногда и банят за них)

        /*
         Решил выбрать только жанры, потому что тегов нет в SteamAPI (Либо я просто их не нашел), их можно получить только при парсинге HTML страницы игры
         Я подумал что сосредоточусь только на взаимодействии со SteamAPI, потому что как мне кажется для тестового хватит информации и статистики по жанрам
         Если данная идея вас не устроила можете засчитать это как замечание
        */
        public List<string> Genres { get; set; }
    }
}
