using System;

namespace MafiaManager.Shared.Enums
{
    /// <summary>
    /// Фаза игры
    /// </summary>
    [Flags]
    public enum GameState
    {
        Day, // День
        Night, // Ночь
        Vote // Голосование
    }
}
