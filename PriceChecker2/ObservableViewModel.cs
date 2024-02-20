using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PriceChecker2;

public abstract class ObservableViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new(propertyName));

    protected void SetValue<T>(ref T _property, T value, [CallerMemberName] string name = "")
    {
        _property = value;
        OnPropertyChanged(name);
    }
}