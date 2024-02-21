using CommunityToolkit.Maui.Core.Extensions;
using PriceChecker2.Saving;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace PriceChecker2.Parts;

public class PartDatabase : Singleton<PartDatabase>
{
    public delegate void DatabaseChangeDelegate(PartInfo part);
    public event DatabaseChangeDelegate? OnPartRegistered;
    public event DatabaseChangeDelegate? OnPartUnregistered;

    private readonly ObservableCollection<PartInfo> _partInfos;
    public ReadOnlyObservableCollection<PartInfo> Parts { get; }
    public bool AllLoaded => !Parts.Any(p => p.Loading);

    public PartDatabase()
    {
        _partInfos = Saver.Instance.State.Parts.Select(p => new PartInfo(p)).ToObservableCollection();
        Parts = new(_partInfos);
    }

    public async Task RegisterAsync(Part part)
    {
        Saver.Instance.State.Parts.Add(part);
        await Saver.Instance.SaveAsync();

        PartInfo partInfo = new(part);
        await AsyncUtils.WaitWhile(() => partInfo.Loading);
        _partInfos.Add(partInfo);

        OnPartRegistered?.Invoke(partInfo);
    }

    public async Task UnregisterAsync(Part part)
    {
        int index = Saver.Instance.State.Parts.IndexOf(part);
        Saver.Instance.State.Parts.RemoveAt(index);
        await Saver.Instance.SaveAsync();

        PartInfo partInfo = _partInfos[index];
        _partInfos.RemoveAt(index);
        
        OnPartUnregistered?.Invoke(partInfo);
    }

    public async Task ClearAsync()
    {
        Saver.Instance.State.Parts.Clear();
        await Saver.Instance.SaveAsync();

        foreach (var partInfo in _partInfos)
            OnPartUnregistered?.Invoke(partInfo);
        _partInfos.Clear();
    }
}
