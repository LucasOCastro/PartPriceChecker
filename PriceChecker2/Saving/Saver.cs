using System.Text.Json;

namespace PriceChecker2.Saving;

public class Saver : Singleton<Saver>
{
    private const string FileName = "parts.json";
    private static readonly string Dir = Path.Combine(FileSystem.Current.AppDataDirectory, FileName);

    public SaveState State { get; } = new();
    
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    public async Task SaveAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            await File.WriteAllTextAsync(Dir, JsonSerializer.Serialize(State));
        }
        finally
        {
            _semaphore.Release();
        }
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