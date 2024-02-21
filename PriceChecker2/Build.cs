using PriceChecker2.Parts;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace PriceChecker2;

public class Build: ObservableViewModel
{
    private readonly ObservableCollection<PartInfo> _buildParts;
    public ReadOnlyObservableCollection<PartInfo> BuildParts { get; }

    public double TotalValidPrice => BuildParts.Where(bp => bp.IsValid).Sum(bp => bp.LowestPrice ?? 0);

    public Build()
    {
        _buildParts = new();
        BuildParts = new(_buildParts);
        _buildParts.CollectionChanged += (s,e) => OnPropertyChanged(nameof(TotalValidPrice));

        PartDatabase.Instance.OnPartRegistered += OnPartRegistered;
        PartDatabase.Instance.OnPartUnregistered += OnPartUnregistered;
        foreach (var part in PartDatabase.Instance.Parts)
        {
            if (part.IsBuildPart)
                _buildParts.Add(part);
            part.PropertyChanged += OnPartPropertyChanged;
        }
    }

    private void OnPartRegistered(PartInfo part)
    {
        if (part.IsBuildPart) _buildParts.Add(part);
        part.PropertyChanged += OnPartPropertyChanged;
    }

    private void OnPartUnregistered(PartInfo part)
    {
        if (part.IsBuildPart) _buildParts.Remove(part);
        part.PropertyChanged -= OnPartPropertyChanged;
    }

    private void OnPartPropertyChanged(object? sender, EventArgs e)
    {
        if (sender is not PartInfo part) return;
        if (e is not PropertyChangedEventArgs { PropertyName: nameof(PartInfo.IsBuildPart) }) return;

        if (part.IsBuildPart) _buildParts.Add(part);
        else _buildParts.Remove(part);
    }
}
