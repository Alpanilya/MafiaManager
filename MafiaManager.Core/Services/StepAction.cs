using MafiaManager.Core.Entities;
using MafiaManager.Core.Interfaces;
using MafiaManager.Shared.Enums;
using System;

namespace MafiaManager.Core.Services
{
    public abstract class StepAction(IGameLogService gameLogService) : IStepAction
    {
        protected readonly IGameLogService _gameLogService = gameLogService ?? 
            throw new ArgumentNullException(nameof(gameLogService));

        public Game Process(Game game, int? nominatedPlayer)
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

                this.ExecuteAction(game, nominatedPlayer);

                return game;
            }
            finally
            {
                if (gameAction != null)
                    game.Actions.Add(gameAction);

                this._gameLogService?.Log(game, gameAction);
            }
        }

        protected abstract void ExecuteAction(Game game, int? nominatedPlayer);
    }
}
