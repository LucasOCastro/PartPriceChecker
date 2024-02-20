using CommunityToolkit.Maui.Core.Extensions;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace PriceChecker2.Parts;

public class PartDatabase : Singleton<PartDatabase>
{
    public delegate void DatabaseChangeDelegate(PartInfo part);
    public event DatabaseChangeDelegate? OnPartRegistered;
    public event DatabaseChangeDelegate? OnPartUnregistered;

    private const string FileName = "parts.json";
    private static readonly string Dir = Path.Combine(FileSystem.Current.AppDataDirectory, FileName);

    private bool _busy;

    private readonly ObservableCollection<PartInfo> _partInfos;
    public ReadOnlyObservableCollection<PartInfo> Parts { get; }
    public bool AllLoaded => !Parts.Any(p => p.Loading);

    private readonly List<Part> _parts;
    public PartDatabase()
    {
        if (!File.Exists(Dir))
        {
            _parts = new();
            File.WriteAllText(Dir, JsonSerializer.Serialize(_parts));
        }
        else _parts = JsonSerializer.Deserialize<List<Part>>(File.ReadAllText(Dir)) ?? new();
        
        _partInfos = _parts.Select(p => new PartInfo(p)).ToObservableCollection();
        Parts = new(_partInfos);
    }

    public async Task SaveChangesAsync()
    {
        if (_busy)
            await AsyncUtils.WaitWhile(() => _busy);

        _busy = true;
        await File.WriteAllTextAsync(Dir, JsonSerializer.Serialize(_parts));
        _busy = false;
    }

    public async Task RegisterAsync(Part part)
    {
        _parts.Add(part);
        await SaveChangesAsync();

        PartInfo partInfo = new(part);
        await AsyncUtils.WaitWhile(() => partInfo.Loading);
        _partInfos.Add(partInfo);

        OnPartRegistered?.Invoke(partInfo);
    }

    public async Task UnregisterAsync(Part part)
    {
        int index = _parts.IndexOf(part);
        _parts.RemoveAt(index);
        await SaveChangesAsync();

        PartInfo partInfo = _partInfos[index];
        _partInfos.RemoveAt(index);
        
        OnPartUnregistered?.Invoke(partInfo);
    }

    public async Task ClearAsync()
    {
        _parts.Clear();
        await SaveChangesAsync();

        foreach (var partInfo in _partInfos)
            OnPartUnregistered?.Invoke(partInfo);
        _partInfos.Clear();
    }
}
