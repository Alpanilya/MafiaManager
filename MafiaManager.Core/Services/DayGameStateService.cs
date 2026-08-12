using MafiaManager.Core.Entities;
using MafiaManager.Core.Interfaces;
using MafiaManager.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MafiaManager.Core.Services
{
    public class VoteGameStateService : GameStateService
    {
        public override Game Process(StateContext context, IGameLogService logService)
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

            logService.Log(game, gameAction);

            this.TryEndPhase(game, logService);

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

        protected override bool TryEndPhase(Game game, IGameLogService logService)
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


        public override void EnterPhase(Game game, IGameLogService logService)
        {
            return;
        }
    }

    /// <summary>
    /// Сервис обработки фазы дневного голосования
    /// </summary>
    public class DayGameStateService : GameStateService
    {
        public override Game Process(StateContext context, IGameLogService logService)
        {
            if (context is not NominateContext nominateContext)
                throw new ArgumentException($"Ожидался {nameof(NominateContext)}", nameof(context));

            Game game = context.Game;

            int? nominatedPlayer = nominateContext.NomitantedPlayer;

            if (this.TryEndPhase(game, logService))
                return game;

            if (!game.CurrentActivePlayer.HasValue)
                return game;

            PlayerCollection players = game.GameSettings.Players;

            Player player = players[game.CurrentActivePlayer.Value];

            GameActionBuilder gameActionBuilder = null;

            if (!nominatedPlayer.HasValue)
            {
                gameActionBuilder = GameActionBuilder.Create(game)
                    .SetPlayer(game.CurrentActivePlayer)
                    .SetActionType(ActionType.Skip)
                    .SetTarget(nominatedPlayer);
            }
            else
            {
                Player targetPlayer = players[nominatedPlayer.Value];

                if (targetPlayer.State == PlayerState.Dead)
                    throw new InvalidOperationException($"<b>{targetPlayer.Name}</b> уже мертв и не может быть номинирован!");

                gameActionBuilder = GameActionBuilder.Create(game)
                    .SetPlayer(game.CurrentActivePlayer)
                    .SetActionType(ActionType.Nominate)
                    .SetTarget(nominatedPlayer);

                targetPlayer.State = PlayerState.Nominated;
            }

            game.CurrentActivePlayer = this.GetNextActiveID(game, logService);

            GameAction gameAction = this.CreateAndAddAction(game, gameActionBuilder);

            logService.Log(game, gameAction);

            this.TryEndPhase(game, logService);

            return game;
        }

        protected override bool TryEndPhase(Game game, IGameLogService logService)
        {
            if (game.CurrentActivePlayer == null)
            {
                game.GameState.Phase = GamePhase.Vote;

                GameAction gameAction = this.CreateAndAddAction(game, GameActionBuilder.Create(game)
                    .SetActionType(ActionType.Vote));

                logService?.Log(game, gameAction);

                game.CurrentActivePlayer = null;

                return true;
            }

            return false;
        }

        protected override int? GetNextActiveID(Game game, IGameLogService logService)
        {
            PlayerCollection players = game.GameSettings.Players;

            Player player = game.CurrentActivePlayer.HasValue 
                ? players[game.CurrentActivePlayer.Value] 
                : null; 

            int index = player is null 
                ? 0 
                : players.IndexOf(player);

            GameActionBuilder builder = GameActionBuilder.Create(game)
                .SetPlayer(game.CurrentActivePlayer)
                .SetActionType(ActionType.Handoff);

            try
            {
                for (int i = index; i < players.Count; i++)
                {
                    Player currentPlayer = players.ElementAt(i);

                    if (currentPlayer.ID == game.CurrentActivePlayer)
                        continue;

                    if (currentPlayer.State != PlayerState.Dead)
                    {
                        builder.SetTarget(currentPlayer.ID);

                        return currentPlayer.ID;
                    }
                }

                return null;
            }
            finally
            {
                GameAction gameAction = this.CreateAndAddAction(game, builder);

                logService.Log(game, gameAction);
            }
        }

        public override void EnterPhase(Game game, IGameLogService logService)
        {
            game.CurrentActivePlayer = this.GetNextActiveID(game, logService);
        }
    }
}
