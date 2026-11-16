using MafiaManager.Core.Entities;
using MafiaManager.Core.Interfaces;
using MafiaManager.Core.Utils;
using MafiaManager.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MafiaManager.Core.Services
{
    /// <summary>
    /// Сервис обработки фазы ночного голосования
    /// </summary>
    public class NightGameStateService(IGameLogService gameLogService) : GameStateService(gameLogService)
    {
        private readonly Dictionary<int, IStepAction> _stepActions = new()
        {
            [Constants.MAFIA_ROLE.ID] = new MafiaStepAction(gameLogService),
            [Constants.GOD_FATHER_ROLE.ID] = new GodFatherStepAction(gameLogService),
            [Constants.SHERIFF_ROLE.ID] = new SheriffStepAction(gameLogService)
        };

        public override void Process(StateContext context)
        {
            if (context is not NominateContext nominateContext)
                throw new ArgumentException($"Ожидался {nameof(NominateContext)}", nameof(context));

            Game game = context.Game;

            PlayerCollection players = game.GameSettings.Players;

            int? nominatedPlayer = nominateContext.NomitantedPlayer;

            if (this._stepActions.TryGetValue(game.CurrentActiveRole.Value, out IStepAction stepAction))
                stepAction.Process(game, nominatedPlayer);

            game.CurrentActiveRole = this.GetNextActiveID(game);

            players.ResetSelection();

            this.TryEndPhase(game);
        }

        protected override bool TryEndPhase(Game game)
        {
            if (!game.CurrentActiveRole.HasValue)
            {
                game.GameState.Day++;
                game.GameState.Phase = GamePhase.Day;

                game.CurrentActiveRole = null;

                return true;
            }

            return false;
        }

        protected override int? GetNextActiveID(Game game)
        {
            var roles = game.GameSettings.Roles;

            PlayerCollection players = game.GameSettings.Players;

            Role role = roles[game.CurrentActiveRole.Value];

            int index = roles.IndexOf(role);

            GameActionBuilder gameActionBuilder = GameActionBuilder.Create(game)
                .SetRole(game.CurrentActiveRole)
                .SetActionType(ActionType.Handoff);

            try
            {
                for (int i = index + 1; i < roles.Count; i++)
                {
                    Role currentRole = roles.ElementAt(i);

                    if (currentRole.Phase != GamePhase.Night)
                        continue;

                    int id = currentRole.ID;

                    if (players.GetPlayersAliveByRole(id).Any())
                    {
                        gameActionBuilder.SetTarget(id);

                        return id;
                    }
                }
            }
            finally
            {
                GameAction gameAction = this.CreateAndAddAction(game, gameActionBuilder);

                this._gameLogService.Log(game, gameAction);
            }

            return null;
        }

 
        public override void EnterPhase(Game game)
        {
            PlayerCollection players = game.GameSettings.Players;

            for (int i = 0; i < Constants.MAFIA_ROLES.Length; i++)
            {
                int role = Constants.MAFIA_ROLES[i];

                Player[] playersAlive = [.. players.GetPlayersAliveByRole(role) ];

                if (playersAlive.Length > 0)
                {
                    game.CurrentActiveRole = Constants.MAFIA_ROLE.ID;

                    GameAction gameAction = this.CreateAndAddAction(game, GameActionBuilder.Create(game)
                        .SetRole(game.CurrentActiveRole)
                        .SetActionType(ActionType.PhaseChanged));

                    this._gameLogService?.Log(game, gameAction);

                    return;
                }
            }

            players.ResetSelection();
        }
    }
}
