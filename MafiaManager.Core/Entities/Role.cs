using MafiaManager.Core.Interfaces;
using System.Drawing;

namespace MafiaManager.Core.Entities
{
    public class Role : Entity, IRole
    {
        public Color Color { get; set; }

        public Role()
        {
        }
    }
}
