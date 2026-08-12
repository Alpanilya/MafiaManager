namespace MafiaManager.Shared.Enums
{
    /// <summary>
    /// Типы действий
    /// </summary>
    public enum ActionType
    {
        Check, // Проверяет
        Kill, // Убивает
        Skip, // Пропускает
        Nominate, // Номинирует
        Vote, // Голосует
        PhaseChanged, // Смена фазы
        GameOver, // Игра завершена
        Handoff, // Следующий ход
    }
}
