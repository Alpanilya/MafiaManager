using MafiaManager.Core.Entities;

namespace MafiaManager.Core.Interfaces
{
    public interface IStepAction
    {
        public Game Process(Game game, int? nominatedPlayer, IGameLogService logService);
    }
}
