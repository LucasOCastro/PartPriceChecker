using PriceChecker2.UrlScraping;

namespace PriceChecker2.Parts;

public class PartInfo : ObservableViewModel
{
    public Part Part { get; }
    public string Name => Part.Name;
    public bool IsBuildPart
    {
        get => Part.IsBuildPart;
        set
        {
            if (value == Part.IsBuildPart) return;
            Part.IsBuildPart = value;
            OnPropertyChanged(nameof(IsBuildPart));
            Task.Run(PartDatabase.Instance.SaveChangesAsync);
        }
    }

    public IEnumerable<UrlScrapedData> AllUrlData => _data;

    public bool Loading { get; private set; }

    public bool IsValid => _cheapestData != null;

    private UrlScrapedData? _cheapestData;
    public double? LowestPrice => _cheapestData?.Price;
    public string LowestPriceStoreIconUri => _cheapestData?.WebsiteIconUri ?? "";
    public string PriceString => IsValid ? _cheapestData.PriceString : "INVALID";

    private readonly List<UrlScrapedData> _data = new();
    public PartInfo(Part part)
    {
        Part = part;

        Loading = true;
        Task.Run(() => LoadDataAsync(part.Urls));
    }

    private async Task LoadDataAsync(string[] urls)
    {
        for (int i = 0; i < urls.Length; i++)
        {
            if (!Uri.TryCreate(urls[i], UriKind.Absolute, out var uri)) continue;
            var data = await UrlScraper.Instance.ScrapeAsync(uri);
            if (data == null) continue;

            _data.Add(data);
            if (_cheapestData == null || data.Price < LowestPrice)
                _cheapestData = data;
        }
        Loading = false;
    }

    public override string ToString() => Name;
}