using MafiaManager.Core.Entities;
using MafiaManager.Core.Services;

namespace MafiaManager.Core.Interfaces
{
    /// <summary>
    /// Представление сервиса обработки фаз игры
    /// </summary>
    public interface IGameStateService
    {
        /// <summary>
        /// Выполняет обработку текущей фазы игры
        /// </summary>
        /// <param name="game">Представление игры (<see cref="IGame"/>)</param>
        /// <param name="nominatedPlayers">Кандидат(<see cref="IGame"/>)</param>
        /// <param name="logService">Сервис логирования(<see cref="IGame"/>)</param>
        public Game Process(StateContext context, IGameLogService logService);

        /// <summary>
        /// Выполняет обработку начальных состояний фазы
        /// </summary>
        /// <param name="game">Представление игры (<see cref="IGame"/>)</param>
        /// <param name="logService">Сервис логирования(<see cref="IGame"/>)</param>
        public void EnterPhase(Game game, IGameLogService logService);

        /// <summary>
        /// Проверка завершения игры
        /// </summary>
        /// <param name="game">Представление игры (<see cref="IGame"/>)</param>
        /// <param name="logService">Сервис логирования(<see cref="IGame"/>)</param>
        /// <remarks>Игра считается оконченой согласно официальным правилам турнирной мафии</remarks>
        public void TryGameComplete(Game game, IGameLogService logService);
    }
}
