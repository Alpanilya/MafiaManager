using MafiaManager.Core.Entities;

namespace MafiaManager.Core.Interfaces
{
    public interface IStepAction
    {
        public void Process(Game game, int? nominatedPlayer);
    }
}
