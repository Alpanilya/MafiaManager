using MafiaManager.Core.Interfaces;

namespace MafiaManager.Core.Services
{
    public class GameLogService : IGameLogService
    {
        public void Log(IGame game, string message)
        {
            if (game == null || string.IsNullOrWhiteSpace(message))
                return;

            game.AddLogMessage(message);
        }
    }
}
