using System.Collections.ObjectModel;
using System.Text.Json;

namespace PriceChecker2.Parts;

public class PartDatabase : Singleton<PartDatabase>
{
    private const string FileName = "parts.json";
    private static readonly string Dir = Path.Combine(FileSystem.Current.AppDataDirectory, FileName);

    private bool _busy;

    private readonly ObservableCollection<Part> _parts;
    public ReadOnlyObservableCollection<Part> Parts { get; }

    public PartDatabase()
    {
        if (!File.Exists(Dir))
        {
            _parts = new();
            File.WriteAllText(Dir, JsonSerializer.Serialize(_parts));
        }
        else _parts = new ObservableCollection<Part>(JsonSerializer.Deserialize<List<Part>>(File.ReadAllText(Dir)) ?? new());
        Parts = new(_parts);
    }

    private async Task SaveChangesAsync()
    {
        if (_busy)
            await new Task(async () =>
            {
                while (_busy)
                    await Task.Delay(100);
            });

        _busy = true;
        await File.WriteAllTextAsync(Dir, JsonSerializer.Serialize(_parts));
        _busy = false;
    }

    public async Task RegisterAsync(Part part)
    {
        _parts.Add(part);
        await SaveChangesAsync();
    }

    public async Task UnregisterAsync(Part part)
    {
        _parts.Remove(part);
        await SaveChangesAsync();
    }

    public async Task ClearAsync()
    {
        _parts.Clear();
        await SaveChangesAsync();
    }
}
