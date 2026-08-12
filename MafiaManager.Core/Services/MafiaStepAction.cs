using MafiaManager.Core.Entities;
using MafiaManager.Core.Interfaces;
using MafiaManager.Shared.Enums;

namespace MafiaManager.Core.Services
{
    public class MafiaStepAction : StepAction
    {
        protected override void ExecuteAction(Game game, int? nominatedPlayer, IGameLogService logService)
        {
            GameAction gameAction = GameActionBuilder.Create(game)
                .SetRole(game.CurrentActiveRole)
                .SetActionType(ActionType.Kill)
                .SetTarget(nominatedPlayer)
                .Build();

            game.Actions.Add(gameAction);

            Player player = game.GameSettings.Players[nominatedPlayer.Value];

            player.State = PlayerState.Dead;

            logService.Log(game, gameAction);
        }
    }
}
