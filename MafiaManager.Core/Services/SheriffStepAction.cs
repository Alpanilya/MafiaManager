using MafiaManager.Core.Entities;
using MafiaManager.Core.Interfaces;
using MafiaManager.Shared.Enums;

namespace MafiaManager.Core.Services
{
    public class SheriffStepAction(IGameLogService gameLogService) : StepAction(gameLogService)
    {
        protected override void ExecuteAction(Game game, int? nominatedPlayer)
        {
            ActionResult result = this.Validate(game, nominatedPlayer);

            PlayerCollection players = game.GameSettings.Players;

            Player player = players.GetOrDefault(nominatedPlayer);

            bool? isMafia = player is not null ? players.IsMafia(player) : null;

            GameAction gameAction = GameActionBuilder.Create(game)
                .SetRole(game.CurrentActiveRole)
                .SetActionType(ActionType.Check)
                .SetTarget(nominatedPlayer)
                .SetActionResult(result)
                .SetCheckResult(isMafia)
                .Build();

            game.Actions.Add(gameAction);

            this._gameLogService.Log(game, gameAction);
        }
    }
}
