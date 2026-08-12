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
            { GamePhase.Day, new DayGameStateService() },
            { GamePhase.Night, new NightGameStateService() },
            { GamePhase.Vote, new VoteGameStateService() },
            { GamePhase.Meeting, new MeetingGameStateService() },
        };

        public Game ProcessPhase(StateContext stateContext)
        {
            Game game = stateContext.Game ?? 
                throw new ArgumentNullException(nameof(stateContext.Game));

            GameState gameState = game.GameState;

            if (gameState.Phase == GamePhase.GameOver)
                throw new InvalidOperationException("Игра была завершена");

            GamePhase prevPhase = gameState.Phase;

            if (!this._services.TryGetValue(game.GameState.Phase, out IGameStateService service))
                throw new InvalidOperationException($"Не найден сервис для фазы {gameState.Phase}");

            game = service.Process(stateContext, this._logService);

            service.TryGameComplete(game, logService);

            if (gameState.Phase == GamePhase.GameOver)
                return game;

            GamePhase newPhase = game.GameState.Phase;

            if (newPhase != prevPhase && this._services.TryGetValue(newPhase, out service))
                service.EnterPhase(game, logService);

            return game;
        }
    }
}
