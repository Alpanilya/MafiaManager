using MafiaManager.Core.Entities;
using MafiaManager.Core.Interfaces;
using MafiaManager.Shared.Enums;

namespace MafiaManager.Core.Services
{
    public class GodFatherStepAction(IGameLogService gameLogService) : StepAction(gameLogService)
    {
        protected override void ExecuteAction(Game game, int? nominatedPlayer)
        {
            PlayerCollection players = game.GameSettings.Players;

            Player player = players[nominatedPlayer.Value];

            bool isSheriff = players.IsSheriff(player);

            GameAction gameAction = GameActionBuilder.Create(game)
                .SetRole(game.CurrentActiveRole)
                .SetActionType(ActionType.Check)
                .SetTarget(nominatedPlayer)
                .SetActionResult(isSheriff)
                .Build();

            game.Actions.Add(gameAction);

            this._gameLogService.Log(game, gameAction);
        }
    }
}
