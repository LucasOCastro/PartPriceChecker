using PriceChecker2.Saving;
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
            Task.Run(Saver.Instance.SaveAsync);
        }
    }

    public int BuildPriority
    {
        get => Part.BuildPriority;
        set
        {
            if (value == Part.BuildPriority) return;
            Part.BuildPriority = value;
            OnPropertyChanged(nameof(BuildPriority));
            Task.Run(Saver.Instance.SaveAsync);
        }
    }

    public IEnumerable<UrlScrapedData> AllUrlData => _data;

    public bool Loading { get; private set; }

    public bool IsValid => _cheapestData != null;

    private UrlScrapedData? _cheapestData;
    public double LowestPrice => _cheapestData?.Price ?? 0;
    public string LowestPriceStoreIconUri => _cheapestData?.WebsiteIconUri ?? "";
    public string LowestPriceDomainName => _cheapestData?.DomainName ?? "";
    public string PriceString => IsValid ? _cheapestData.PriceString : "INVALID";
    

    private readonly List<UrlScrapedData> _data = new();
    public PartInfo(Part part)
    {
        Part = part;

        Loading = true;
        Task.Run(() => LoadDataAsync(part.Urls));
    }

    private async Task LoadDataAsync(IEnumerable<string> urls)
    {
        foreach (var url in urls)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)) continue;
            var data = await UrlScraper.Instance.ScrapeAsync(uri);
            if (data != null) _data.Add(data);
        }
        _cheapestData = AllUrlData.MinBy(data => data.Price);
        Loading = false;
    }

    public async Task ChangePartData(string newName, IEnumerable<string> newUrls)
    {
        Part.Name = newName;
        Part.Urls = newUrls.ToArray();
        await Task.Run(Saver.Instance.SaveAsync);

        //Removes all the data from _data that do not exist in newUrls
        _data.RemoveAll(data => !newUrls.Contains(data.Url));
        //Loads all the newUrls that werent in _data before (updates the cheapest inside)
        await LoadDataAsync(newUrls.Where(newUrl => !_data.Any(data => data.Url == newUrl)));
    }

    private bool _affordable;
    public bool Affordable 
    {
        get => _affordable;
        set => SetValue(ref _affordable, value);
    }

    public override string ToString() => Name;
}