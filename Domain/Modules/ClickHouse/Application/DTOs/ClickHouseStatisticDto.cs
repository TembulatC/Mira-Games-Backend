// DTO для добавления снимка данных статистики в ClickHouse
namespace Domain.Modules.ClickHouse.Application.DTOs
{
    public record class ClickHouseStatisticDto
    {
        public DateTime date { get; set; }
        public PopularGenres[] popularGenres { get; set; }
    }

    public record class PopularGenres
    {
        public string genre { get; set; }

        public int games { get; set; }
    }
}
