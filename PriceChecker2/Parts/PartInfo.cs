using PriceChecker2.UrlScraping;

namespace PriceChecker2.Parts;

public class PartInfo
{
    private readonly Part _part;
    public string Name => _part.Name;
    public bool IsBuildPart
    {
        get => _part.IsBuildPart;
        set
        {
            if (value == _part.IsBuildPart) return;
            _part.IsBuildPart = value;
            Task.Run(PartDatabase.Instance.SaveChangesAsync);
        }
    }

    public bool Loading { get; private set; }

    public bool IsValid => _cheapestData != null;

    private UrlScrapedData? _cheapestData;
    public double? LowestPrice => _cheapestData?.Price;
    public string LowestPriceStoreIconUri => _cheapestData?.WebsiteIconUri ?? "";
    public string PriceString => IsValid ? string.Format("{0:C}", LowestPrice) : "INVALID";

    private readonly List<UrlScrapedData> _data = new();
    public PartInfo(Part part)
    {
        _part = part;

        Loading = true;
        Task.Run(() => LoadDataAsync(part.Urls));
    }

    private async Task LoadDataAsync(string[] urls)
    {
        for (int i = 0; i < urls.Length; i++)
        {
            var data = await UrlScraper.Instance.ScrapeAsync(new Uri(urls[i]));
            if (data == null) continue;

            _data.Add(data);
            if (_cheapestData == null || data.Price < LowestPrice)
                _cheapestData = data;
        }
        Loading = false;
    }

    public override string ToString() => Name;
}