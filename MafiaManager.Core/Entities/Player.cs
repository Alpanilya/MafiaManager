using MafiaManager.Shared.Enums;

namespace MafiaManager.Core.Entities
{
    public class Player : Entity<int>
    {
        public int Role { get; set; }

        public PlayerState State { get; set; }

        public bool IsSelected { get; set; }
    }
}
