namespace MafiaManager.Core.Entities
{
    public class GameSettings
    {
        public PlayerCollection Players { get; set; }

        public EntityCollection<Role, int> Roles { get; set; }

        public int TimeDelay { get; set; }

        public int DefaultRole { get; set; }
    }
}
