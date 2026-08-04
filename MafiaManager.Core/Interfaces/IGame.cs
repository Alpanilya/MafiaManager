using MafiaManager.Core.Converters;
using MafiaManager.Shared.Enums;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MafiaManager.Core.Interfaces
{
    /// <summary>
    /// Представление игры
    /// </summary>
    [JsonConverter(typeof(GameConverter))]
    public interface IGame : IEntity
    {
        /// <summary>
        /// Настройки текущей игры
        /// </summary>
        public IGameSettings GameSettings { get; }

        /// <summary>
        ///  Фаза игры
        /// </summary>
        public GameState GameState { get; set; }

        /// <summary>
        /// Список игров
        /// </summary>
        public IList<IPlayer> Players { get; set; }
    }
}
