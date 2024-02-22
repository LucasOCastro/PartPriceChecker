using PriceChecker2.Parts;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace PriceChecker2;

public class Build: ObservableViewModel
{
    private readonly ObservableCollection<PartInfo> _buildParts;
    public ReadOnlyObservableCollection<PartInfo> BuildParts { get; }

    public double TotalValidPrice => BuildParts.Where(bp => bp.IsValid).Sum(bp => bp.LowestPrice);

    private void Add(PartInfo part)
    {
        if (part.BuildPriority >= 0)
        {
            _buildParts.InsertOrderedBy(part, bp => bp.BuildPriority);
            bool test = _buildParts.DistinctBy(bp => bp.BuildPriority).Count() == _buildParts.Count;
        }
        else
        {
            part.BuildPriority = _buildParts.Count;
            _buildParts.Add(part);
        }
    }

    private void Remove(PartInfo part)
    {
        part.BuildPriority = -1;
        _buildParts.Remove(part);
    }

    public void SwapBuildPriorities(int a, int b)
    {
        if (a == b) return;
        _buildParts[a].BuildPriority = b;
        _buildParts[b].BuildPriority = a;
        _buildParts.Swap(a, b);
    }

    public Build()
    {
        _buildParts = new();
        BuildParts = new(_buildParts);
        _buildParts.CollectionChanged += (s,e) => OnPropertyChanged(nameof(TotalValidPrice));

        PartDatabase.Instance.OnPartRegistered += OnPartRegistered;
        PartDatabase.Instance.OnPartUnregistered += OnPartUnregistered;

        LoadPartsAsync();
    }

    private async Task LoadPartsAsync()
    {
        await AsyncUtils.WaitUntil(() => PartDatabase.Instance.AllLoaded);
        foreach (var part in PartDatabase.Instance.Parts)
            OnPartRegistered(part);
    }

    private void OnPartRegistered(PartInfo part)
    {
        if (part.IsBuildPart) Add(part);
        part.PropertyChanged += OnPartPropertyChanged;
    }

    private void OnPartUnregistered(PartInfo part)
    {
        if (part.IsBuildPart) Remove(part);
        part.PropertyChanged -= OnPartPropertyChanged;
    }

    private void OnPartPropertyChanged(object? sender, EventArgs e)
    {
        if (sender is not PartInfo part) return;
        if (e is not PropertyChangedEventArgs args) return;

        switch(args.PropertyName)
        {
            case nameof(PartInfo.IsBuildPart):
                if (part.IsBuildPart) Add(part);
                else Remove(part);
                break;
            case nameof(PartInfo.LowestPrice):
                OnPropertyChanged(nameof(TotalValidPrice));
                break;
        }
    }
}
