using MafiaManager.Core.Entities;

namespace MafiaManager.Core.Interfaces
{
    public interface IGameActionMessage
    {
        public string Format(Game game, GameAction action);
    }

}
