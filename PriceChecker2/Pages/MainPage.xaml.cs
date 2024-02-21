using PriceChecker2.Parts;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace PriceChecker2.Pages;

public partial class MainPage : ContentPage
{
    private readonly ObservableCollection<PartInfo> _buildParts = new();

    public MainPage()
    {
        InitializeComponent();
        LoadPartInfoAsync();
    }

    private async Task LoadPartInfoAsync()
    {
        IsBusy = true;

        await AsyncUtils.WaitUntil(() => PartDatabase.Instance.AllLoaded);

        foreach (var part in PartDatabase.Instance.Parts)
        {
            if (part.IsBuildPart)
                _buildParts.Add(part);
            part.PropertyChanged += OnPartPropertyChanged;
        }
        _collectionView.ItemsSource = _buildParts;

        PartDatabase.Instance.OnPartRegistered += OnPartRegistered;
        PartDatabase.Instance.OnPartUnregistered += OnPartUnregistered;

        IsBusy = false;
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
        if (e is not PropertyChangedEventArgs { PropertyName: nameof(PartInfo.IsBuildPart)}) return;

        if (part.IsBuildPart) _buildParts.Add(part);
        else _buildParts.Remove(part);
    }
}