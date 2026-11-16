using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MafiaManager.Core.Entities
{
    public class EntityCollection<TEntity, TKey> : KeyedCollection<TKey, TEntity> 
        where TEntity : Entity<TKey>
    {
        // Сразу вызываем создание словаря параметром dictionaryCreationThreshold = 0
        public EntityCollection() : base(null, 0) { }

        protected override TKey GetKeyForItem(TEntity item) => 
            item.ID;

        public TEntity ElementAt(int index)
        {
            if (index < 0 || index > this.Count - 1)
                return null;

            return this[index];
        }
    }

    public static class EntityCollectionExtensions
    {
        public static TEntity GetOrDefault<TEntity, TKey>(this EntityCollection<TEntity, TKey> collection, TKey? key)
            where TKey : struct
            where TEntity : Entity<TKey> =>
            key.HasValue && collection.Contains(key.Value)
                ? collection[key.Value]
                : null;
    }   

    public class Entity<TKey> : INotifyPropertyChanged
    {
        public TKey ID { get; set; }

        public string Name { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string propertyName = default) =>
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public bool Set<T>(ref T field, T value, [CallerMemberName] string propertyName = default)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;

            this.OnPropertyChanged(propertyName);

            return true;
        }
    }
}
