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

        public GameLogMessage AddLogMessage(string message, GameAction action)
        {
            GameLogMessage log = new()
            {
                ID = Guid.NewGuid(),
                Date = DateTime.Now,
                Phase = this.GameState.Phase,
                Day = this.GameState.Day,
                Message = message,
                OwnerID = action?.ID
            };

            this.Log.Add(log);

            return log;
        }
    }
}