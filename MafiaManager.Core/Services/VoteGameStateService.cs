using MafiaManager.Core.Entities;
using MafiaManager.Core.Interfaces;
using MafiaManager.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MafiaManager.Core.Services
{
    /// <summary>
    /// Сервис обработки фазы голосования на исключение
    /// </summary>
    public class VoteGameStateService(IGameLogService gameLogService) : GameStateService(gameLogService)
    {
        public override Game Process(StateContext context)
        {
            if (context is not VoteContext voteContext)
                throw new ArgumentException($"Ожидался {nameof(VoteContext)}", nameof(context));

            Game game = context.Game;

            IReadOnlyDictionary<int, int?> votes = voteContext.Votes;

            if (GetEliminatedPlayer(voteContext) is not Player player)
                throw new InvalidOperationException($"Игроки не приняли единогласного решения!");

            GameAction gameAction = this.CreateAndAddAction(game, GameActionBuilder.Create(game)
                .SetActionType(ActionType.Kill)
                .SetTarget(player.ID));

            player.State = PlayerState.Dead;

            this._gameLogService.Log(game, gameAction);

            this.TryEndPhase(game);

            return game;
        }

        private static Player GetEliminatedPlayer(VoteContext voteContext)
        {
            var votes = voteContext.Votes;

            int? max = votes.Values.Max();

            if (!max.HasValue || max <= 0)
                return null;

            KeyValuePair<int, int?>[] pairs = [.. votes.Where(v => v.Value == max)];

            if (pairs.Length != 1)
                return null;

            PlayerCollection players = voteContext.Game.GameSettings.Players;

            return players[pairs[0].Key];
        }

        protected override bool TryEndPhase(Game game)
        {
            PlayerCollection players = game.GameSettings.Players;

            // Сброс состояний игрока
            foreach (Player player in players)
            {
                if (player.State != PlayerState.Nominated)
                    continue;

                player.State = PlayerState.Alive;
            }

            game.GameState.Phase = GamePhase.Night;

            return true;
        }


        public override void EnterPhase(Game game)
        {
            game.GameSettings.Players.ResetSelection();

            return;
        }
    }
}
