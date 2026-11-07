using MafiaManager.Core.Utils;
using MafiaManager.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MafiaManager.Core.Entities
{
    public class PlayerCollection : EntityCollection<Player, int>
    {
        public PlayerCollection() : base() { }

        public IEnumerable<Player> GetPlayersByRole(int roleId) => roleId < 0
            ? throw new ArgumentOutOfRangeException(nameof(roleId))
            : this.Where(p => p.Role == roleId);

        public IEnumerable<Player> GetPlayersAliveByRole(int roleId) => roleId < 0
            ? throw new ArgumentOutOfRangeException(nameof(roleId))
            : this.GetPlayersByRole(roleId).Where(p => p.State != PlayerState.Dead);

        public IEnumerable<Player> GetPlayersAlive() => 
            this.Where(p => p.State != PlayerState.Dead);

        public bool IsMafia(Player player) =>
            Constants.MAFIA_ROLES.Contains(player.Role);

        public bool IsSheriff(Player player) =>
            Constants.SHERIFF_ROLE.ID == player.Role;

        public Player GetLastAlive()
        {
            for (int i = this.Count - 1; i >= 0; i--)
            {
                if (this[i].State != PlayerState.Dead)
                    return this[i];
            }

            return null;
        }

        public void ResetSelection()
        {
            foreach (Player player in this)
            {
                player.IsSelected = false;
            }
        }
    }

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
