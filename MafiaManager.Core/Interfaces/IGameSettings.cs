using System.Collections.Generic;

namespace MafiaManager.Core.Interfaces
{
    /// <summary>
    /// Представление настроек игры
    /// </summary>
    public interface IGameSettings
    {
        /// <summary>
        /// Время, отведеннное на голосование
        /// </summary>
        public int TimeDelay { get; set; }

        /// <summary>
        /// Кол-во игроков
        /// </summary>
        public int PlayerCount { get; set; }

        /// <summary>
        /// Список доступных ролей игроков
        /// </summary>
        public IList<IRole> Roles {  get; set; }
    }
}
