using MafiaManager.Core.Converters;
using System.Text.Json.Serialization;

namespace MafiaManager.Core.Interfaces
{
    /// <summary>
    /// Представление роли
    /// </summary>
    [JsonConverter(typeof(RoleConverter))]
    public interface IRole : IEntity
    {
        /// <summary>
        /// Цвет роли
        /// </summary>
        public string Color { get; set; }
    }
}
