using MafiaManager.Core.Entities;
using MafiaManager.Core.Interfaces;
using MafiaManager.Core.Utils;
using MafiaManager.Shared.Enums;

namespace MafiaManager.Core.Services
{
    public class GodFatherStepAction: StepAction
    {
        protected override void ExecuteAction(Game game, int? nominatedPlayer, IGameLogService logService)
        {
            PlayerCollection players = game.GameSettings.Players;

            Player player = players[nominatedPlayer.Value];

            bool isSheriff = player.Role == Constants.SHERIFF_ROLE.ID;

            GameAction gameAction = GameActionBuilder.Create(game)
                .SetRole(game.CurrentActiveRole)
                .SetActionType(ActionType.Check)
                .SetTarget(nominatedPlayer)
                .SetActionResult(isSheriff)
                .Build();

            game.Actions.Add(gameAction);

            logService.Log(game, gameAction);
        }
    }
}
