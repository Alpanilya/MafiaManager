using MafiaManager.Core.Interfaces;
using MafiaManager.Shared.Enums;
using System;
using System.Collections.Generic;

namespace MafiaManager.Core.Entities
{
    public class Game : Entity, IGame
    {
        public IGameSettings GameSettings { get; set; }

        public GameState GameState { get; set; }

        public IList<IPlayer> Players { get; set; }

        public int Day { get; set; }

        private readonly List<IGameLogMessage> _log = [];

        public IReadOnlyCollection<IGameLogMessage> Log { get; }

        public Game()
        {
            this.Log = this._log.AsReadOnly();
        }

        public void AddLogMessage(string message)
        {
            this._log.Add(new GameLogMessage
            {
                Date = DateTime.Now,
                GameState = this.GameState,
                Message = message
            });
        }
    }
}
