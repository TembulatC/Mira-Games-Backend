using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MiraGamesBackend.Utilities
{
    public static class GameGenre
    {
        [Display(Name = "Action")]
        public const string Action = "Action";

        [Display(Name = "RPG")]
        public const string RPG = "RPG";

        [Display(Name = "Strategy")]
        public const string Strategy = "Strategy";

        [Display(Name = "Simulation")]
        public const string Simulation = "Simulation";

        [Display(Name = "Adventure")]
        public const string Adventure = "Adventure";

        [Display(Name = "Indie")]
        public const string Indie = "Indie";

        [Display(Name = "Casual")]
        public const string Casual = "Casual";

        [Display(Name = "Horror")]
        public const string Horror = "Horror";

        [Display(Name = "Shooter")]
        public const string Shooter = "Shooter";

        [Display(Name = "Racing")]
        public const string Racing = "Racing";

        [Display(Name = "Massively Multiplayer")]
        public const string MassivelyMultiplayer = "Massively Multiplayer";

        [Display(Name = "Free To Play")]
        public const string FreeToPlay = "Free To Play";

        [Display(Name = "Early Access")]
        public const string EarlyAccess = "Early Access";

        // Метод для получения всех жанров
        public static string[] GetAllGenres()
        {
            return
            [
                Action, RPG, Strategy, Simulation, Adventure,
                Indie, Casual, Horror, Shooter, Racing,
                MassivelyMultiplayer, FreeToPlay, EarlyAccess
            ];
        }
    }

    public static class SupportPlatforms
    {
        [Display(Name = "Windows")]
        public const string Windows = "Windows";

        [Display(Name = "Mac")]
        public const string Mac = "Mac";

        [Display(Name = "Linux")]
        public const string Linux = "Linux";

        // Метод для получения всех платформ
        public static string[] GetAllSupportPlatforms()
        {
            return
            [
                Windows, Mac, Linux
            ];
        }
    }
}