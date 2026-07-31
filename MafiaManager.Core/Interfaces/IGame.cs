using MafiaManager.Shared.Enums;
using System.Collections.Generic;

namespace MafiaManager.Core.Interfaces
{
    /// <summary>
    /// Представление игры
    /// </summary>
    public interface IGame : IEntity
    {
        /// <summary>
        /// Время, отведеннное на голосование
        /// </summary>
        public int TimeDelay { get; set; }

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
