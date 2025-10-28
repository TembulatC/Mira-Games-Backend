using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel.DataAnnotations;

namespace MiraGamesBackend.Utilities
{
    public class GameGenreParameterFilter : IParameterFilter
    {
        public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
        {
            if (parameter.Name.Equals("genre", StringComparison.OrdinalIgnoreCase))
            {
                // Устанавливаем тип string и добавляем значения
                parameter.Schema.Type = "string";
                parameter.Schema.Enum.Clear();

                foreach (var genre in GameGenre.GetAllGenres())
                {
                    parameter.Schema.Enum.Add(new OpenApiString(genre));
                }
            }
        }
    }

    public class GameSupportPlatformsParameterFilter : IParameterFilter
    {
        public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
        {
            if (parameter.Name.Equals("supportPlatforms", StringComparison.OrdinalIgnoreCase))
            {
                // Устанавливаем тип string и добавляем значения
                parameter.Schema.Type = "string";
                parameter.Schema.Enum.Clear();

                foreach (var genre in SupportPlatforms.GetAllSupportPlatforms())
                {
                    parameter.Schema.Enum.Add(new OpenApiString(genre));
                }
            }
        }
    }
}