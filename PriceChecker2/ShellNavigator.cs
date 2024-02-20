namespace PriceChecker2;

internal class ShellNavigator : Singleton<ShellNavigator>
{
    private readonly List<string> _stack = new();
    private string _current = "";

    private void PushCurrent()
    {
        int foundIndex = _stack.IndexOf(_current);
        if (foundIndex != -1) _stack.RemoveAt(foundIndex);
        _stack.Add(_current);
    }

    public ShellNavigator()
    {
        Shell.Current.Navigated += (o, e) => _current = e.Current.Location.ToString();
        _current = Shell.Current.CurrentState.Location.ToString();
    }

    public async Task NavigateAsync(string path, Dictionary<string, object>? parameters = null)
    {
        PushCurrent();
        if (parameters == null)
            await Shell.Current.GoToAsync(path);
        else
            await Shell.Current.GoToAsync(path, parameters);
    }

    public async Task BackAsync()
    {
        if (_stack.Count == 0) return;

        int lastIndex = _stack.Count - 1;
        string target = _stack[lastIndex];
        _stack.RemoveAt(lastIndex);

        await Shell.Current.GoToAsync(target);
    }
}
