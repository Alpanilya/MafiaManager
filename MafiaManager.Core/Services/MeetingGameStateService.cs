using MafiaManager.Core.Entities;
using MafiaManager.Core.Interfaces;
using MafiaManager.Shared.Enums;

namespace MafiaManager.Core.Services
{
    /// <summary>
    /// Сервис обработки фазы знакомства
    /// </summary>
    public class MeetingGameStateService : GameStateService
    {
        public override Game Process(StateContext context, IGameLogService logService)
        {
            Game game = context.Game;

            this.TryEndPhase(game, logService);

            return game;
        }

        protected override bool TryEndPhase(Game game, IGameLogService logService)
        {
            game.GameState.Phase = GamePhase.Night;

            game.CurrentActivePlayer = null;

            return true;
        }

        public override void EnterPhase(Game game, IGameLogService logService)
        {
            game.GameSettings.Players.ResetSelection();

            return;
        }
    }
}
