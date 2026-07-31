using MafiaManager.Core.Interfaces;
using MafiaManager.Shared.Enums;

namespace MafiaManager.Core.Entities
{
    public class Player : Entity, IPlayer
    {
        public IRole Role { get; set; }

        public PlayerState State { get; set; }
    }
}
