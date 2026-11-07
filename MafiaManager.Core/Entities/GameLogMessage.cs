using MafiaManager.Shared.Enums;
using System;

namespace MafiaManager.Core.Entities
{
    public class GameLogMessage : Entity<Guid>
    {
        public DateTime Date { get; set; }

        /// <summary>
        ///  Фаза игры
        /// </summary>
        public GamePhase Phase { get; set; }

        /// <summary>
        /// Текущий день
        /// </summary>
        public int Day { get; set; }

        public string Message { get; set; }

        public Guid? OwnerID { get; set; }
    }
}
