using MafiaManager.Shared.Enums;

namespace MafiaManager.Core.Interfaces
{
    /// <summary>
    /// Представление игрока
    /// </summary>
    public interface IPlayer : IEntity
    {
        /// <summary>
        /// Роль игрока
        /// </summary>
        public IRole Role { get; set; }

        /// <summary>
        /// Текущее состояние игрока
        /// </summary>
        public PlayerState State { get; set; }
    }
}
