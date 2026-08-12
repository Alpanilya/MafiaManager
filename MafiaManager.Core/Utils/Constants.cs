using MafiaManager.Core.Entities;
using MafiaManager.Shared.Enums;

namespace MafiaManager.Core.Utils
{
    public class Constants
    {
        public static readonly Role MAFIA_ROLE = new() { ID = 1, Name = "Мафия", Color = "#000000", Phase = GamePhase.Night };

        public static readonly Role SHERIFF_ROLE = new() { ID = 2, Name = "Шериф", Color = "#FFFF00", Phase = GamePhase.Night };

        public static readonly Role GOD_FATHER_ROLE = new() { ID = 3, Name = "Дон", Color = "#000000", Phase = GamePhase.Night };

        public static readonly Role TOWNSMAN_ROLE = new () { ID = 4, Name = "Мирный", Color = "#E63946", Phase = GamePhase.Day };

        public static readonly int[] MAFIA_ROLES =
        [
            Constants.MAFIA_ROLE.ID,
            Constants.GOD_FATHER_ROLE.ID
        ];
    }
}
