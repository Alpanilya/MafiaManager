using MafiaManager.Core.Entities;
using MafiaManager.Core.Interfaces;
using MafiaManager.Shared.Enums;
using System;
using System.Collections.Generic;

namespace MafiaManager.Core.Services
{
    /// <summary>
    /// Сервис обработки фаз игры
    /// </summary>
    public class GameStateStrategy(IGameLogService logService)
    {
        private readonly Dictionary<GamePhase, IGameStateService> _services = new()
        {
            { GamePhase.Day, new DayGameStateService(logService) },
            { GamePhase.Night, new NightGameStateService(logService) },
            { GamePhase.Vote, new VoteGameStateService(logService) },
            { GamePhase.Meeting, new MeetingGameStateService(logService) },
        };

        public IReadOnlyList<PhaseTransition> ProcessPhase(StateContext stateContext)
        {
            ArgumentNullException.ThrowIfNull(stateContext.Game);

            Game game = stateContext.Game;

            GameState gameState = game.GameState;

            GamePhase handledPhase = gameState.Phase;

            int handledDay = game.GameState.Day;

            if (gameState.Phase == GamePhase.GameOver)
                throw new InvalidOperationException("Игра была завершена");

            if (!this._services.TryGetValue(game.GameState.Phase, out IGameStateService service))
                throw new InvalidOperationException($"Не найден сервис для фазы {gameState.Phase}");

            List<PhaseTransition> transitions = [];

            service.Process(stateContext);

            // Защита от зацикливания. Можем сделать не более чем полный круг переключений
            for (int i = 0; i < Enum.GetNames<GamePhase>().Length; i++)
            {
                GamePhase current = game.GameState.Phase;

                service.TryGameComplete(game);

                if (gameState.Phase == GamePhase.GameOver)
                    break;

                if (current == handledPhase)
                    break;

                transitions.Add(new(handledPhase, current, handledDay));

                if (handledPhase != current && this._services.TryGetValue(current, out service))
                    service.EnterPhase(game);

                handledPhase = current;
            }

            return transitions;
        }

        public void ProcessSwitch(StateContext stateContext)
        {
            ArgumentNullException.ThrowIfNull(stateContext.Game);

            Game game = stateContext.Game;

            GameState gameState = game.GameState;

            if (gameState.Phase == GamePhase.GameOver)
                throw new InvalidOperationException("Игра была завершена");

            if (!this._services.TryGetValue(game.GameState.Phase, out IGameStateService service))
                throw new InvalidOperationException($"Не найден сервис для фазы {gameState.Phase}");

            service.ProcessSwitch(stateContext);
        }
    }
}
