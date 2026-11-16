using MafiaManager.Shared.Enums;
using System;

namespace MafiaManager.Core.Entities
{
    public class GamePhaseSummary : Entity<Guid>
    {
        public GamePhase Phase { get; set; }

        public int Day { get; set; }

        public string Message { get; set; }
    }
}
