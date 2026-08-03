using MafiaManager.Core.Converters;
using System.Collections.Generic;
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
        public IList<IPlayer> Players { get; set; }

        /// <summary>
        /// Список доступных ролей игроков
        /// </summary>
        public IList<IRole> Roles {  get; set; }
    }
}
