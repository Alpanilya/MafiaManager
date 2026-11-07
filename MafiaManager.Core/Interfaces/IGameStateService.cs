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
        /// <param name="StateContext">Контекст состояния (<see cref="StateContext"/>)</param>
        /// <param name="logService">Сервис логирования(<see cref="IGameLogService"/>)</param>
        public Game Process(StateContext context, IGameLogService logService);

        /// <summary>
        /// Выполняет обработку начальных состояний фазы
        /// </summary>
        /// <param name="game">Представление игры (<see cref="Game"/>)</param>
        /// <param name="logService">Сервис логирования(<see cref="IGameLogService"/>)</param>
        public void EnterPhase(Game game, IGameLogService logService);

        /// <summary>
        /// Проверка завершения игры
        /// </summary>
        /// <param name="game">Представление игры (<see cref="IGame"/>)</param>
        /// <param name="logService">Сервис логирования(<see cref="IGameLogService"/>)</param>
        /// <remarks>Игра считается оконченой согласно официальным правилам турнирной мафии</remarks>
        public void TryGameComplete(Game game, IGameLogService logService);

        /// <summary>
        /// Выполняет обработку переключения между действующими игроками текущей фазы
        /// </summary>
        /// <param name="StateContext">Контекст состояния (<see cref="StateContext"/>)</param>
        /// <param name="logService">Сервис логирования(<see cref="IGameLogService"/>)</param>
        public Game ProcessSwitch(StateContext context, IGameLogService logService);
    }
}
