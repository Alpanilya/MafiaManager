using MafiaManager.Core.Interfaces;
using MafiaManager.Shared.Enums;
using System;

namespace MafiaManager.Core.Services
{
    /// <summary>
    /// Сервис обработки фазы ночного голосования
    /// </summary>
    [Obsolete("Не финальная версия")]
    public class NightVoteGameStateService : IGameStateService
    {
        public GameState Process(IGame game)
        {
            return GameState.Day;
        }
    }
}
