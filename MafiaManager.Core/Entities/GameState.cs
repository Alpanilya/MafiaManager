using MafiaManager.Shared.Enums;

namespace MafiaManager.Core.Entities
{
    public class GameState
    {
        /// <summary>
        ///  Фаза игры
        /// </summary>
        public GamePhase Phase { get; set; }

        /// <summary>
        /// Текущий день
        /// </summary>
        public int Day { get; set; }
    }
}
