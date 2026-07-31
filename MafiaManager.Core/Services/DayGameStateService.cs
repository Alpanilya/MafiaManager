using MafiaManager.Core.Interfaces;
using MafiaManager.Shared.Enums;
using System;

namespace MafiaManager.Core.Services
{
    /// <summary>
    /// Сервис обработки дневной фазы игры
    /// </summary>
    [Obsolete("Не финальная версия")]
    public class DayGameStateService : IGameStateService
    {
        public GameState Process(IGame game)
        {
            return GameState.Day | GameState.Vote;
        }
    }
}
