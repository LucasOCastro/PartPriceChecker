
namespace PriceChecker2;

public class Observable<T> : ObservableViewModel
{
    private T _value;
    public T Value 
    {
        get => _value;
        set => SetValue(ref _value, value);
    }

    public Observable(T value)
    {
        Value = value;
    }
    public Observable() : this(default) { }

    public static implicit operator T(Observable<T> observable) => observable.Value;
    public static implicit operator Observable<T>(T value) => new(value);
}
