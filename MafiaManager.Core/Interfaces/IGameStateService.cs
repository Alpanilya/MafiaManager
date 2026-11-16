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
        /// <param name="context">Контекст состояния (<see cref="StateContext"/>)</param>
        public void Process(StateContext context);

        /// <summary>
        /// Выполняет обработку начальных состояний фазы
        /// </summary>
        /// <param name="game">Представление игры (<see cref="Game"/>)</param>
        public void EnterPhase(Game game);

        /// <summary>
        /// Проверка завершения игры
        /// </summary>
        /// <param name="game">Представление игры (<see cref="IGame"/>)</param>
        /// <remarks>Игра считается оконченой согласно официальным правилам турнирной мафии</remarks>
        public void TryGameComplete(Game game);

        /// <summary>
        /// Выполняет обработку переключения между действующими игроками текущей фазы
        /// </summary>
        /// <param name="context">Контекст состояния (<see cref="StateContext"/>)</param>
        public void ProcessSwitch(StateContext context);
    }

    /// <summary>
    /// Представляет сервис обработки сводной информации по фазе игры
    /// </summary>
    public interface IGamePhaseSummaryService
    {
        /// <summary>
        /// Создание сводной информации по фазе игры
        /// </summary>
        /// <param name="context">Контекст состояния (<see cref="StateContext"/>)</param>
        public GamePhaseSummary ProcessSummary(SummaryContext context);
    }
}
