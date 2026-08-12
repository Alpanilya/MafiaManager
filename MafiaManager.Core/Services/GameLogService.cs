using MafiaManager.Core.Entities;
using MafiaManager.Core.Interfaces;

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

            game.AddLogMessage(message);

            this._notificationService?.NotifyAsync(game.ID.ToString(), message);
        }
    }
}
