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
        private readonly IGameLogService _logService = logService ?? throw new ArgumentNullException(nameof(logService));

        private readonly Dictionary<GamePhase, IGameStateService> _services = new()
        {
            { GamePhase.Day, new DayGameStateService(logService) },
            { GamePhase.Night, new NightGameStateService(logService) },
            { GamePhase.Vote, new VoteGameStateService(logService) },
            { GamePhase.Meeting, new MeetingGameStateService(logService) },
        };

        public Game ProcessPhase(StateContext stateContext)
        {
            ArgumentNullException.ThrowIfNull(stateContext.Game);

            Game game = stateContext.Game;

            GameState gameState = game.GameState;

            if (gameState.Phase == GamePhase.GameOver)
                throw new InvalidOperationException("Игра была завершена");

            GamePhase prevPhase = gameState.Phase;

            if (!this._services.TryGetValue(game.GameState.Phase, out IGameStateService service))
                throw new InvalidOperationException($"Не найден сервис для фазы {gameState.Phase}");

            game = service.Process(stateContext);

            service.TryGameComplete(game);

            if (gameState.Phase == GamePhase.GameOver)
                return game;

            GamePhase newPhase = game.GameState.Phase;

            if (newPhase != prevPhase && this._services.TryGetValue(newPhase, out service))
                service.EnterPhase(game);

            return game;
        }

        public Game ProcessSwitch(StateContext stateContext)
        {
            ArgumentNullException.ThrowIfNull(stateContext.Game);

            Game game = stateContext.Game;

            GameState gameState = game.GameState;

            if (gameState.Phase == GamePhase.GameOver)
                throw new InvalidOperationException("Игра была завершена");

            if (!this._services.TryGetValue(game.GameState.Phase, out IGameStateService service))
                throw new InvalidOperationException($"Не найден сервис для фазы {gameState.Phase}");

            game = service.ProcessSwitch(stateContext);

            return game;
        }

    }
}
