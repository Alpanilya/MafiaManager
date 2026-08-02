using MafiaManager.Core.Converters;
using System.Text.Json.Serialization;

namespace MafiaManager.Core.Interfaces
{
    /// <summary>
    /// Представление настроек игры
    /// </summary>

    [JsonConverter(typeof(GameSettingsConverter))]
    public interface IGameSettings
    {
        /// <summary>
        /// Время, отведеннное на голосование
        /// </summary>
        public int TimeDelay { get; set; }

        /// <summary>
        /// Список игроков
        /// </summary>
        public IPlayer[] Players { get; set; }

        /// <summary>
        /// Список доступных ролей игроков
        /// </summary>
        public IRole[] Roles {  get; set; }
    }
}
