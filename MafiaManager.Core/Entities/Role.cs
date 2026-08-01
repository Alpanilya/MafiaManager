using MafiaManager.Core.Interfaces;

namespace MafiaManager.Core.Entities
{
    public class Role : Entity, IRole
    {
        public string Color { get; set; }
    }
}
