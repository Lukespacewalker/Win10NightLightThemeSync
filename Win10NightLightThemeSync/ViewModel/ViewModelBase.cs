using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Win10NightLightThemeSync.ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void SetAndRaiseIfChanged<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(backingField,value))
            { 
                backingField = value;
                OnPropertyChanged(propertyName);
            }

        }
    }
}