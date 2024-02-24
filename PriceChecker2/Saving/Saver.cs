using System.Text.Json;

namespace PriceChecker2.Saving;

public class Saver : Singleton<Saver>
{
    private const string FileName = "parts.json";
    private static readonly string Dir = Path.Combine(FileSystem.Current.AppDataDirectory, FileName);

    public SaveState State { get; } = new();

    private bool _busy;

    public async Task SaveAsync()
    {
        //Should I be using "lock"?
        if (_busy)
            await AsyncUtils.WaitWhile(() => _busy);

        _busy = true;
        await File.WriteAllTextAsync(Dir, JsonSerializer.Serialize(State));
        _busy = false;
    }

    public Saver()
    {
        if (!File.Exists(Dir))
        {
            File.WriteAllText(Dir, JsonSerializer.Serialize(State));
        }
        else State = JsonSerializer.Deserialize<SaveState>(File.ReadAllText(Dir)) ?? new();
    }
}