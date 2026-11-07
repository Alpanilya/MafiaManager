using System;
using System.Collections.Generic;

namespace MafiaManager.Core.Entities
{
    public class Game : Entity<Guid>
    {
        public GameSettings GameSettings { get; set; }

        public GameState GameState { get; set; }

        public IList<GameLogMessage> Log { get; } = [];

        public DateTime DateLastConnection { get; set; }

        public int? CurrentActiveRole { get; set; }

        public int? CurrentActivePlayer { get; set; }

        public GameActionCollection Actions { get; } = [];

        public void AddLogMessage(string message)
        {
            this.Log.Add(new GameLogMessage
            {
                Date = DateTime.Now,
                Phase = this.GameState.Phase,
                Day = this.GameState.Day,
                Message = message
            });
        }
    }
}
