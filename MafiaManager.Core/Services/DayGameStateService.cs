using MafiaManager.Core.Entities;
using MafiaManager.Core.Interfaces;
using MafiaManager.Shared.Enums;
using System;
using System.Linq;

namespace MafiaManager.Core.Services
{
    /// <summary>
    /// Сервис обработки фазы наминации на выбывание
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

            GameState gameState = game.GameState;

            GameActionCollection actions = game.Actions;

            GameAction gameAction = actions.GetActions(gameState, player.ID, ActionType.Nominate);

            Player targetPlayer = players[nominatedPlayer.Value];

            if (targetPlayer.State == PlayerState.Dead)
                throw new InvalidOperationException($"<b>{targetPlayer.Name}</b> уже мертв и не может быть номинирован!");

            if (gameAction != null)
            {
                if (gameAction.TargetID.HasValue)
                {
                    Player undoPlayer = players[gameAction.TargetID.Value];

                    undoPlayer.State = PlayerState.Alive;
                }

                gameAction.TargetID = targetPlayer.ID;

                targetPlayer.State = PlayerState.Nominated;
            }
            else
            {
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
                    gameActionBuilder = GameActionBuilder.Create(game)
                        .SetPlayer(game.CurrentActivePlayer)
                        .SetActionType(ActionType.Nominate)
                        .SetTarget(nominatedPlayer);

                    targetPlayer.State = PlayerState.Nominated;
                }

                game.CurrentActivePlayer = this.GetNextActiveID(game, logService);

                gameAction = this.CreateAndAddAction(game, gameActionBuilder);
            }

            logService.Log(game, gameAction);

            players.ResetSelection();

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
            PlayerCollection players = game.GameSettings.Players;

            players.ResetSelection();

            game.CurrentActivePlayer = this.GetNextActiveID(game, logService);
        }

        public override Game ProcessSwitch(StateContext context, IGameLogService logService)
        {
            if (context is not SwitchContext switchContext)
                throw new ArgumentException($"Ожидался {nameof(SwitchContext)}", nameof(context));

            Game game = context.Game;

            GameState gameState = game.GameState;

            if (gameState?.Phase != GamePhase.Day)
                return game;

            GameActionCollection actions = game.Actions;

            PlayerCollection players = game.GameSettings.Players;

            Player[] playersAlive = [.. players.GetPlayersAlive()];

            Player currentPlayer = players[game.CurrentActivePlayer.Value];

            // Действия номинаций и пропуска ходов
            GameAction[] nominatedActions = actions.GetActions(gameState, ActionType.Nominate, ActionType.Skip);

            int cursor = switchContext.Cursor;

            int start = Array.IndexOf(playersAlive, currentPlayer);

            // Сдвиг должен осуществлять на живого игрока
            int next = Math.Clamp(start + cursor, 0, playersAlive.Length - 1);

            Player nextPlayer = playersAlive.ElementAt(next);

            int? nextActivePlayer = nextPlayer?.ID;

            if (nextActivePlayer.HasValue && nominatedActions.Length > 0)
            {
                players.ResetSelection();

                GameAction action = nominatedActions.FirstOrDefault(p => p.PlayerID == nextPlayer.ID);

                // Выбор на игрока, которого номинировал игрок
                if (action != null && action.TargetID.HasValue)
                {
                    Player player = players[action.TargetID.Value];

                    player.IsSelected = true;
                }
            }

            if (nextActivePlayer.HasValue)
                game.CurrentActivePlayer = nextActivePlayer;

            return game;
        }
    }
}
