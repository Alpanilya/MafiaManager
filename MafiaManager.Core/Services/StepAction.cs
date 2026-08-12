using MafiaManager.Core.Entities;
using MafiaManager.Core.Interfaces;
using MafiaManager.Shared.Enums;

namespace MafiaManager.Core.Services
{
    public abstract class StepAction : IStepAction
    {
        public Game Process(Game game, int? nominatedPlayer, IGameLogService logService)
        {
            GameAction gameAction = null;

            try
            {
                if (!nominatedPlayer.HasValue)
                {
                    gameAction = GameActionBuilder.Create(game)
                        .SetRole(game.CurrentActiveRole)
                        .SetActionType(ActionType.Skip)
                        .SetTarget(nominatedPlayer)
                        .Build();

                    return game;
                }

                if (!game.GameSettings.Players.Contains(nominatedPlayer.Value))
                {
                    gameAction = GameActionBuilder.Create(game)
                        .SetRole(game.CurrentActiveRole)
                        .SetActionType(ActionType.Skip)
                        .SetTarget(nominatedPlayer)
                        .SetActionResult(ActionResult.Fail)
                        .Build();

                    return game;
                }

                this.ExecuteAction(game, nominatedPlayer, logService);

                return game;
            }
            finally
            {
                if (gameAction != null)
                    game.Actions.Add(gameAction);

                logService?.Log(game, gameAction);
            }
        }

        protected abstract void ExecuteAction(Game game, int? nominatedPlayer, IGameLogService logService);
    }
}
