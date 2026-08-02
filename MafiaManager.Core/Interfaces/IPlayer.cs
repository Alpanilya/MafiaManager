using MafiaManager.Core.Converters;
using MafiaManager.Shared.Enums;
using System.Text.Json.Serialization;

namespace MafiaManager.Core.Interfaces
{
    /// <summary>
    /// Представление игрока
    /// </summary>
    [JsonConverter(typeof(PlayerConverter))]
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
