using MafiaManager.Shared.Enums;
using System;
using System.Collections.Generic;

namespace MafiaManager.Core.Interfaces
{
    /// <summary>
    /// Представление сервиса обработки фаз игры
    /// </summary>
    public interface IGameStateService
    {
        /// <summary>
        /// Выполняет обработку текущей фазы игры и возвращает следующую фазу
        /// </summary>
        /// <param name="game">Представление игры (<see cref="IGame"/>)</param>
        /// <returns>Следующая фаза игры</returns>
        public GameState Process(IGame game);
    }
}
