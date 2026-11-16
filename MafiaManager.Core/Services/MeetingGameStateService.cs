using MafiaManager.Core.Entities;
using MafiaManager.Core.Interfaces;
using MafiaManager.Shared.Enums;

namespace MafiaManager.Core.Services
{
    /// <summary>
    /// Сервис обработки фазы знакомства
    /// </summary>
    public class MeetingGameStateService(IGameLogService gameLogService) : GameStateService(gameLogService)
    {
        public override void Process(StateContext context)
        {
            Game game = context.Game;

            this.TryEndPhase(game);
        }

        protected override bool TryEndPhase(Game game)
        {
            game.GameState.Phase = GamePhase.Night;

            game.CurrentActivePlayer = null;

            return true;
        }

        public override void EnterPhase(Game game)
        {
            game.GameSettings.Players.ResetSelection();

            return;
        }
    }
}
