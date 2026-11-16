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
        public override void Process(StateContext context)
        {
            if (context is not VoteContext voteContext)
                throw new ArgumentException($"Ожидался {nameof(VoteContext)}", nameof(context));

            Game game = context.Game;

            IReadOnlyDictionary<int, int?> votes = voteContext?.Votes;

            GameActionBuilder gameActionBuilder = null;

            if (GetEliminatedPlayer(voteContext) is not Player player)
                throw new InvalidOperationException($"Игроки не приняли единогласного решения!");

            gameActionBuilder = GameActionBuilder.Create(game)
                .SetActionType(ActionType.Vote)
                .SetActionResult(ActionResult.Success)
                .SetTarget(player.ID);

            player.State = PlayerState.Dead;
            
            GameAction gameAction = this.CreateAndAddAction(game, gameActionBuilder);

            this._gameLogService.Log(game, gameAction);

            this.TryEndPhase(game);
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

            players.ResetSelection();

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
            Player[] players = [.. game.GameSettings.Players.FilterBy(new() { States = [PlayerState.Nominated] })];

            GameActionBuilder actionBuilder = GameActionBuilder.Create(game)
                .SetActionType(ActionType.Vote);

            if (players.Length == 0)
            {
                actionBuilder.SetActionResult(ActionResult.Skip);

                this.TryEndPhase(game);
            }
            else if (players.Length == 1)
            {
                Player player = players[0];

                actionBuilder
                    .SetActionResult(ActionResult.Success)
                    .SetTarget(player.ID);

                player.State = PlayerState.Dead;

                this.TryEndPhase(game);
            }

            GameAction action = this.CreateAndAddAction(game, actionBuilder);

            this._gameLogService.Log(game, action);

            game.GameSettings.Players.ResetSelection();

            return;
        }
    }
}
