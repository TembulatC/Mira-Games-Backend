// DTO для данных календаря игр по месяцам
namespace Domain.Modules.DataProcessing.Application.DTOs
{
    public record class CalendarDto
    {
        public string month { get; set; }

        public Days[] days { get; set; }
    }

    public record class Days
    {
        public DateOnly date { get; set; }

        public int count { get; set; }
    }
}
