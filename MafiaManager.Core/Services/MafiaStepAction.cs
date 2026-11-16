using MafiaManager.Core.Entities;
using MafiaManager.Core.Interfaces;
using MafiaManager.Shared.Enums;

namespace MafiaManager.Core.Services
{
    public class MafiaStepAction(IGameLogService gameLogService) : StepAction(gameLogService)
    {
        protected override void ExecuteAction(Game game, int? nominatedPlayer)
        {
            ActionResult result = this.Validate(game, nominatedPlayer);

            GameAction gameAction = GameActionBuilder.Create(game)
                .SetRole(game.CurrentActiveRole)
                .SetActionType(ActionType.Kill)
                .SetTarget(nominatedPlayer)
                .SetActionResult(result)
                .Build();

            game.Actions.Add(gameAction);

            Player player = game.GameSettings.Players.GetOrDefault(nominatedPlayer);

            player?.State = PlayerState.Dead;

            this._gameLogService.Log(game, gameAction);
        }
    }
}
