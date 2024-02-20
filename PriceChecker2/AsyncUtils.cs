namespace PriceChecker2;

public static class AsyncUtils
{
    public static async Task WaitUntil(Func<bool> predicate, int ms = 50)
    {
        while (!predicate())
            await Task.Delay(ms);
    }

    public static async Task WaitWhile(Func<bool> predicate, int ms = 50)
    {
        while (predicate())
            await Task.Delay(ms);
    }
}
