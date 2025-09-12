
namespace PriceChecker2;

public partial class Observable<T>(T value) : ObservableViewModel
{
    private T _value = value;
    public T Value 
    {
        get => _value;
        set => SetValue(ref _value, value);
    }

    public static implicit operator T(Observable<T> observable) => observable.Value;
    public static implicit operator Observable<T>(T value) => new(value);
}
