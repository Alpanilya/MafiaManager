using System.Drawing;

namespace MafiaManager.Core.Interfaces
{
    /// <summary>
    /// Представление роли
    /// </summary>
    public interface IRole : IEntity
    {
        /// <summary>
        /// Цвет роли
        /// </summary>
        public Color Color { get; set; }
    }
}
