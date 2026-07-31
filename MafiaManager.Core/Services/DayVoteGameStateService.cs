using MafiaManager.Core.Interfaces;
using MafiaManager.Shared.Enums;
using System;

namespace MafiaManager.Core.Services
{
    /// <summary>
    /// Сервис обработки фазы дневного голосования
    /// </summary>
    [Obsolete("Не финальная версия")]
    public class DayVoteGameStateService : IGameStateService
    {
        public GameState Process(IGame game)
        {
            return GameState.Night;
        }
    }
}
