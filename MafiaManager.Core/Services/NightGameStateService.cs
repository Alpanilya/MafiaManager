using MafiaManager.Core.Interfaces;
using MafiaManager.Shared.Enums;
using System;

namespace MafiaManager.Core.Services
{
    /// <summary>
    /// Сервис обработки ночного фазы игры
    /// </summary>
    [Obsolete("Не финальная версия")]
    public class NightGameStateService : IGameStateService
    {
        public GameState Process(IGame game)
        {
            return GameState.Night | GameState.Vote;
        }
    }
}
