using MafiaManager.Shared.Enums;

namespace MafiaManager.Core.Interfaces
{

    public interface IPlayer : IEntity
    {
        public IRole Role { get; set; }


        public PlayerState State { get; set; }
    }
}
