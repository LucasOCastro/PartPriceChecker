using PriceChecker2.UrlScraping;

namespace PriceChecker2;

public class Part
{
    public string Name { get; set; } = "New Part";
    public string[] Urls { get; set; } = Array.Empty<string>();

    public override string ToString() => Name;
}

public class PartInfo 
{
    public string Name { get; }

    public bool Loading { get; private set; }
    public bool IsValid => _cheapestData != null;

    private UrlScrapedData? _cheapestData;
    public double? LowestPrice => _cheapestData?.Price;
    public string LowestPriceStoreIconUri => _cheapestData?.WebsiteIconUri ?? "";
    public string PriceString => IsValid ? string.Format("{0:C}", LowestPrice) : "INVALID";
    
    private readonly List<UrlScrapedData> _data = new();
    public PartInfo(Part part)
    {
        Name = part.Name;
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