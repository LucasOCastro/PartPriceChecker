using System.Collections.ObjectModel;

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
        BindingContext = this;
		InitializeComponent();
        _partsCollectionView.ItemsSource = _scrapedParts;
        Task.Run(() => LoadPartInfoAsync(PartDatabase.Instance.Parts));
    }

    private void ClearButton_Pressed(object sender, EventArgs e)
    {
        Task.Run(() =>PartDatabase.Instance.ClearAsync());
    }
}