using MafiaManager.Core.Interfaces;
using System.Collections.Generic;

namespace MafiaManager.Core.Entities
{
    public class GameSettings : IGameSettings
    {
        public IList<IPlayer> Players { get; set; }

        public IList<IRole> Roles { get; set; }

        public int TimeDelay { get; set; }
    }
}
