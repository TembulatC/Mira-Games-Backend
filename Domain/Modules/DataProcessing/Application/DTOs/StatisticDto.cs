// DTO для получения статистики популярных жанров
namespace Domain.Modules.DataProcessing.Application.DTOs
{
    public record class StatisticDto
    {
        public string genre { get; set; }

        public int games { get; set; }
    }
}
