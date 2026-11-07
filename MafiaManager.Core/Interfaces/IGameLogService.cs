using MafiaManager.Core.Entities;

namespace MafiaManager.Core.Interfaces
{
    public interface IGameLogService
    {
        public void Log(Game game, GameAction action);

        /// <summary>
        /// Перестраивает лог игры, начиная с указанного действия. Используется при откате хода.
        /// </summary>
        /// <param name="game">Игра</param>
        /// <param name="cursor">Действие, с которого начинать перестройку</param>
        public void Rebuild(Game game, GameAction cursor);
    }
}
