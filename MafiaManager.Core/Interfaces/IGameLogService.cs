using MafiaManager.Core.Entities;

namespace MafiaManager.Core.Interfaces
{
    public interface IGameLogService
    {
        public void Log(Game game, GameAction action);
    }
}
