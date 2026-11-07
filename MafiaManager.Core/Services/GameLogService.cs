using MafiaManager.Core.Entities;
using MafiaManager.Core.Interfaces;
using System;
using System.Linq;

namespace MafiaManager.Core.Services
{
    public class GameLogService(INotificationLogService notificationService, IGameActionMessage actionMessage) : IGameLogService
    {
        private readonly INotificationLogService _notificationService = notificationService;

        private readonly IGameActionMessage _gameActionMessage = actionMessage;

        public void Log(Game game, GameAction action)
        {
            if (game == null || action == null)
                return;

            string message = this._gameActionMessage.Format(game, action);

            GameLogMessage log = game.AddLogMessage(message, action);

            this._notificationService?.NotifyAsync(game.ID.ToString(), log);
        }

        public void Rebuild(Game game, GameAction cursor)
        {
            ArgumentNullException.ThrowIfNull(cursor);

            GameActionCollection actions = game.Actions;

            int index = actions.IndexOf(cursor);

            if (index < 0)
                throw new ArgumentException($"Действие {cursor.ID} не найдено в логе игры", nameof(cursor));

            GameLogMessage log = game.Log.FirstOrDefault(l => l.OwnerID == cursor.ID) ?? 
                throw new InvalidOperationException($"Сообщение лога для действия {cursor.ID} не найдено");

            string message = this._gameActionMessage.Format(game, cursor);

            log.Message = message;

            this._notificationService?.NotifyUpdateAsync(game.ID.ToString(), log);
        }
    }
}
