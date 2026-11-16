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

        public void Process(Game game, int? nominatedPlayer) =>
            this.ExecuteAction(game, nominatedPlayer);

        protected abstract void ExecuteAction(Game game, int? nominatedPlayer);

        protected virtual ActionResult Validate(Game game, int? nominatedPlayer)
        {
            if (!nominatedPlayer.HasValue)
            {
                return ActionResult.Skip;
            }
            else if (game.GameSettings.Players.GetOrDefault(nominatedPlayer) is null)
            {
                return ActionResult.Fail;
            }

            return ActionResult.Success;
        }
    }
}
