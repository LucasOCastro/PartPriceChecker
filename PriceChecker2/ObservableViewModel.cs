using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PriceChecker2;

public abstract class ObservableModelView : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void SetValue<T>(ref T _property, T value, [CallerMemberName] string name = "")
    {
        _property = value;
        PropertyChanged?.Invoke(this, new(name));
    }
}