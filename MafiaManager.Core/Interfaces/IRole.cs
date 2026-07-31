using System.Drawing;

namespace MafiaManager.Core.Interfaces
{
    public interface IRole : IEntity
    {
        public Color Color { get; set; }
    }
}
