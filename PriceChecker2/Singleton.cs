namespace PriceChecker2;

public abstract class Singleton<T> where T : Singleton<T>, new()
{
    public static readonly T Instance = new();
}
