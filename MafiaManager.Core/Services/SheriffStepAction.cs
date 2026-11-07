using MafiaManager.Core.Entities;
using MafiaManager.Core.Interfaces;
using MafiaManager.Core.Utils;
using MafiaManager.Shared.Enums;
using System;
using System.Linq;

namespace MafiaManager.Core.Services
{
    public class SheriffStepAction : StepAction
    {
        protected override void ExecuteAction(Game game, int? nominatedPlayer, IGameLogService logService)
        {
            PlayerCollection players = game.GameSettings.Players;

            Player player = players[nominatedPlayer.Value];

            bool isMafia = players.IsMafia(player);

            GameAction gameAction = GameActionBuilder.Create(game)
                .SetRole(game.CurrentActiveRole)
                .SetActionType(ActionType.Check)
                .SetTarget(nominatedPlayer)
                .SetActionResult(isMafia)
                .Build();

            game.Actions.Add(gameAction);

            logService.Log(game, gameAction);
        }
    }
}
