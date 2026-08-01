using MafiaManager.Core.Interfaces;

namespace MafiaManager.Core.Entities
{
    public class GameSettings : IGameSettings
    {
        public int PlayerCount { get; set; }

        public IRole[] Roles { get; set; }

        public int TimeDelay { get; set; }
    }
}
