using System;

namespace MafiaManager.Shared.Enums
{
    /// <summary>
    /// Фаза игры
    /// </summary>
    [Flags]
    [Obsolete("Возможно понадобится уход от Flags. После финальной версии удалить аттрибут")]
    public enum GameState
    {
        Day = 1, // День
        Night = 2, // Ночь
        Vote = 4 // Голосование
    }
}
