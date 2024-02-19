using System.Collections.ObjectModel;

namespace PriceChecker2;

public partial class PartTrackerPage : ContentPage
{
    private readonly ObservableCollection<PartInfo> _scrapedParts = new();

    private async Task LoadPartInfoAsync(IEnumerable<Part> parts)
    {
        var scraped = parts.Select(p => new PartInfo(p)).ToList();
        while (scraped.Any(s => s.Loading))
            await Task.Delay(50);

        foreach (var s in scraped)
            _scrapedParts.Add(s);
    }

    public PartTrackerPage()
	{
		InitializeComponent();
        _partsCollectionView.ItemsSource = _scrapedParts;
        Task.Run(() => LoadPartInfoAsync(PartDatabase.Instance.Parts));
    }
}