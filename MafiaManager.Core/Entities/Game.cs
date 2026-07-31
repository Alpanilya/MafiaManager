using MafiaManager.Core.Interfaces;
using MafiaManager.Shared.Enums;
using System.Collections.Generic;

namespace MafiaManager.Core.Entities
{
    public class Game : Entity, IGame
    {
        public int TimeDelay { get; set; }

        public GameState GameState { get; set; }

        public IList<IPlayer> Players { get; set; }
    }
}
