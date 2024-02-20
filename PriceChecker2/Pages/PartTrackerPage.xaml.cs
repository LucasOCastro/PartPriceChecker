using System.Collections.ObjectModel;
using PriceChecker2.Parts;

namespace PriceChecker2;

public partial class PartTrackerPage : ContentPage
{
    private readonly ObservableCollection<PartInfo> _scrapedParts = new();

    private async Task LoadPartInfoAsync(IEnumerable<Part> parts)
    {
        IsBusy = true;
        var scraped = parts.Select(p => new PartInfo(p)).ToList();
        while (scraped.Any(p => p.Loading))
            await Task.Delay(50);

        foreach (var s in scraped)
            _scrapedParts.Add(s);
        IsBusy = false;
    }

    public PartTrackerPage()
	{
		InitializeComponent();
        BindingContext = this;
        _partsCollectionView.ItemsSource = _scrapedParts;

        LoadPartInfoAsync(PartDatabase.Instance.Parts);
    }
}