using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PriceChecker2;

public abstract class ObservableViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new(propertyName));

    protected void SetValue<T>(ref T property, T value, [CallerMemberName] string name = "")
    {
        property = value;
        OnPropertyChanged(name);
    }
}