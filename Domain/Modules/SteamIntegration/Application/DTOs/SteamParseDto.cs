// DTO для сохранения состояния в "data-state.json" с актуальной станицей парсера в удобном формате
namespace Domain.Modules.SteamIntegration.Application.DTOs
{
    public record class SteamParseDto
    {
        public int startPage { get; set; }
    }
}
