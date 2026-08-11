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
        Day = 0, // День
        Night = 1, // Ночь
    }
}
