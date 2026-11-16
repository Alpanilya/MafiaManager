using MafiaManager.Shared.Enums;

namespace MafiaManager.Core.Entities
{
    public record PhaseTransition
    {
        /// <summary>
        ///  Исходная фаза игры
        /// </summary>
        public GamePhase Phase { get; set; }

        /// <summary>
        ///  Фаза, в которую перешла игра
        /// </summary>
        public GamePhase To { get; set; }

        /// <summary>
        /// День, в которых произошел переход
        /// </summary>
        public int Day { get; set; }

        public PhaseTransition(GamePhase phase, GamePhase to, int day)
        {
            this.Phase = phase;
            this.To = to;
            this.Day = day;
        }
    }

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
