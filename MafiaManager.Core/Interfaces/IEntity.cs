namespace MafiaManager.Core.Interfaces
{
    /// <summary>
    /// Представление сущности
    /// </summary>
    public interface IEntity
    {
        public int ID { get; set; }

        public string Name { get; set; }
    }
}