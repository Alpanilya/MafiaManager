using MafiaManager.Shared.Enums;

namespace MafiaManager.Core.Entities
{
    public class Role : Entity<int>
    {
        public string Color { get; set; }

        public GamePhase Phase { get; set; }
    }
}
