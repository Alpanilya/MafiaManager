using MafiaManager.Core.Interfaces;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace MafiaManager.Core.Entities
{
    public class Entity : IEntity, INotifyPropertyChanged
    {
        public int ID { get; set; }

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
