using System;
using System.Collections.Generic;

namespace ErmineGames.Utils
{
    public class NotifiedProperty<T>
    {
        public event Action<T, T> OnChanged;
        
        private T currentValue;

        public NotifiedProperty() { }

        public NotifiedProperty(T initialValue)
        {
            currentValue = initialValue;
        }
        
        public T Value
        {
            get => currentValue;
            set => Set(value);
        }
        
        private void Set(T newValue)
        {
            if (EqualityComparer<T>.Default.Equals(currentValue, newValue))
            {
                return;
            }
            
            var oldValue = currentValue;
            currentValue = newValue;
            OnChanged?.Invoke(oldValue, newValue);
        }
    }
}
