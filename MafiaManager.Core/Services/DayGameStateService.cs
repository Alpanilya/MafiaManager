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
    public class DayGameStateService(IGameLogService gameLogService) : GameStateService(gameLogService)
    {
        public override Game Process(StateContext context)
        {
            if (context is not NominateContext nominateContext)
                throw new ArgumentException($"Ожидался {nameof(NominateContext)}", nameof(context));

            Game game = context.Game;

            int? nominatedPlayer = nominateContext.NomitantedPlayer;

            if (this.TryEndPhase(game))
                return game;

            if (!game.CurrentActivePlayer.HasValue)
                return game;

            PlayerCollection players = game.GameSettings.Players;

            Player player = players.GetOrDefault(game.CurrentActivePlayer);

            GameState gameState = game.GameState;

            GameActionCollection actions = game.Actions;

            GameAction gameAction = actions.GetAction(gameState, player.ID, ActionType.Nominate, ActionType.Skip);

            Player targetPlayer = players.GetOrDefault(nominatedPlayer);

            if (targetPlayer?.State == PlayerState.Dead)
                throw new InvalidOperationException($"<b>{targetPlayer.Name}</b> мертв и не может быть номинирован!");

            if (gameAction != null)
            {
                if (gameAction.TargetID.HasValue)
                {
                    Player undoPlayer = players.GetOrDefault(gameAction.TargetID.Value);

                    undoPlayer.State = PlayerState.Alive;
                }

                if (gameAction.TargetID.HasValue)
                {
                    gameAction.TargetID = targetPlayer.ID;

                    targetPlayer.State = PlayerState.Nominated;
                }

                this._gameLogService.Rebuild(game, gameAction);

                game.CurrentActivePlayer = this.GetNextActiveID(game);
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

                gameAction = this.CreateAndAddAction(game, gameActionBuilder);

                this._gameLogService.Log(game, gameAction);

                game.CurrentActivePlayer = this.GetNextActiveIDWithLog(game);
            }


            players.ResetSelection();

            this.TryEndPhase(game);

            return game;
        }

        protected override bool TryEndPhase(Game game)
        {
            if (game.CurrentActivePlayer == null)
            {
                game.GameState.Phase = GamePhase.Vote;

                GameAction gameAction = this.CreateAndAddAction(game, GameActionBuilder.Create(game)
                    .SetActionType(ActionType.Vote));

                this._gameLogService.Log(game, gameAction);

                game.CurrentActivePlayer = null;

                return true;
            }

            return false;
        }

        private int? GetNextActiveIDWithLog(Game game)
        {
            GameActionBuilder builder = GameActionBuilder.Create(game)
                .SetPlayer(game.CurrentActivePlayer)
                .SetActionType(ActionType.Handoff);

            int? nextID = this.GetNextActiveID(game);

            builder.SetTarget(nextID);

            GameAction gameAction = this.CreateAndAddAction(game, builder);

            this._gameLogService.Log(game, gameAction);

            return nextID;
        }

        protected override int? GetNextActiveID(Game game)
        {
            PlayerCollection players = game.GameSettings.Players;

            Player[] playersAlive = [.. players.GetPlayersAlive()];

            GameActionCollection gameActions = game.Actions;

            // Переключаемся на игрока, который еще не совершал номинации
            for (int i = 0; i < playersAlive.Length; i++)
            {
                Player currentPlayer = playersAlive[i];

                var actions = gameActions.GetAction(game.GameState, currentPlayer.ID, ActionType.Nominate, ActionType.Skip);

                if (actions != null)
                    continue;

                return currentPlayer.ID;
            }

            return null;
        }

        public override void EnterPhase(Game game)
        {
            PlayerCollection players = game.GameSettings.Players;

            players.ResetSelection();

            game.CurrentActivePlayer = this.GetNextActiveIDWithLog(game);
        }

        public override Game ProcessSwitch(StateContext context)
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

            Player currentPlayer = players.GetOrDefault(game.CurrentActivePlayer);

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
                    Player player = players.GetOrDefault(action.TargetID);

                    player.IsSelected = true;
                }
            }

            if (nextActivePlayer.HasValue)
                game.CurrentActivePlayer = nextActivePlayer;

            return game;
        }
    }
}
