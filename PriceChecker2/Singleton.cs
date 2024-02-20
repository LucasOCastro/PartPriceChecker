namespace PriceChecker2;

public abstract class Singleton<T> where T : Singleton<T>, new()
{
    private static readonly T _instance = new();
    public static T Instance => _instance;
}
