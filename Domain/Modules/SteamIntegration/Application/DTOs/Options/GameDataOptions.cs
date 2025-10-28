namespace Domain.Modules.SteamIntegration.Application.DTOs.Options
{
    public record class GameDataOptions
    {
        public required int? SteamId { get; set; } // Уникальный идентификатор игры в Steam

        public required string Title { get; set; } // Название игры

        public DateTime ReleaseDate { get; set; } // Дата релиза игры

        public List<string> Genres { get; set; } // Спискок жанров

        public string StoreURL { get; set; } // Ссылка на страницу игры в магазине

        public string ImageURL { get; set; } // Ссылка на официальный постер игры

        public string ShortDescription { get; set; } // Короткое описание игры

        public List<string> SupportedPlatforms { get; set; } // Поддерживаемые платформы
    }
}
