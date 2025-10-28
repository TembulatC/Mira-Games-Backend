// DTO для десериализации данных из "gameinfo.json" для работы с базой данных
namespace Domain.Modules.DataProcessing.Application.DTOs
{
    public record class GameDataDBDto
    {
        public int SteamId { get; set; } // Уникальный идентификатор игры в Steam

        public string Title { get; set; } // Название игры

        public DateOnly ReleaseDate { get; set; } // Дата релиза игры

        public List<string> Genres { get; set; } // Спискок жанров

        public string StoreURL { get; set; } // Ссылка на страницу игры в магазине

        public string ImageURL { get; set; } // Ссылка на официальный постер игры

        public string ShortDescription { get; set; } // Короткое описание игры

        public List<string> SupportedPlatforms { get; set; } // Поддерживаемые платформы
    }
}
