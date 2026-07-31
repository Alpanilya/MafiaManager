using MafiaManager.Core.Interfaces;
using MafiaManager.Shared.Enums;
using System;
using System.Collections.Generic;

namespace MafiaManager.Core.Services
{
    /// <summary>
    /// Сервис обработки фаз игры
    /// </summary>
    public class GameStateStrategy
    {
        private readonly Dictionary<GameState, IGameStateService> _services = new()
        {
            { GameState.Day, new DayGameStateService() },
            { GameState.Day | GameState.Vote, new DayVoteGameStateService() },
            { GameState.Night, new NightGameStateService() },
            { GameState.Night | GameState.Vote, new NightVoteGameStateService() }
        };

        public GameState ProcessPhase(IGame game)
        {
            game = game ?? throw new ArgumentNullException(nameof(game));

            if (!this._services.TryGetValue(game.GameState, out IGameStateService service))
                throw new ArgumentOutOfRangeException(nameof(GameState));

            return service.Process(game);
        }
    }
}
