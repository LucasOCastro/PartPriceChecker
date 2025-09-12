using PriceChecker2.Saving;
using PriceChecker2.UrlScraping;
using System.Diagnostics.CodeAnalysis;

namespace PriceChecker2.Parts;

public partial class PartInfo(Part part) : ObservableViewModel
{
    public Part Part { get; } = part;
    public string Name => Part.Name;
    public bool IsBuildPart
    {
        get => Part.IsBuildPart;
        set
        {
            if (value == Part.IsBuildPart) return;
            Part.IsBuildPart = value;
            OnPropertyChanged(nameof(IsBuildPart));
            _ = Saver.Instance.SaveAsync();
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
            _ = Saver.Instance.SaveAsync();
        }
    }

    public IEnumerable<UrlScrapedData> AllUrlData => _data.AsEnumerable();

    private bool _isLoaded;
    public bool IsLoaded
    {
        get => _isLoaded;
        set => SetValue(ref _isLoaded, value);
    }

    [MemberNotNullWhen(returnValue: true, member: nameof(_cheapestData))]
    public bool IsValid => _cheapestData != null;

    private UrlScrapedData? _cheapestData;
    public double LowestPrice => IsValid ? _cheapestData.Price : 0;
    public string LowestPriceStoreIconUri => IsValid ? _cheapestData.WebsiteIconUri : "";
    public string LowestPriceDomainName => IsValid ? _cheapestData.DomainName : "";
    public string PriceString => IsValid ? _cheapestData.PriceString : "INVALID";
    

    private readonly List<UrlScrapedData> _data = new();

    private void RefreshCheapestData()
    {
        _cheapestData = AllUrlData.Where(data => data.IsValid).MinBy(data => data.Price);
        OnPropertyChanged(nameof(IsValid));
        OnPropertyChanged(nameof(LowestPrice));
        OnPropertyChanged(nameof(LowestPriceStoreIconUri));
        OnPropertyChanged(nameof(LowestPriceDomainName));
        OnPropertyChanged(nameof(PriceString));
    }

    public void BeginLoading()
    {
        if (IsLoaded) return;
        _ = LoadDataAsync(Part.Urls.AsEnumerable());
    }

    private async Task LoadDataAsync(IEnumerable<string> urls)
    {
        IsLoaded = true;
        
        try
        {
            await Task.WhenAll(urls.Select(async url =>
            {
                if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
                {
                    var scraped = await UrlScraper.Instance.ScrapeAsync(uri);
                    if (scraped != null)
                        _data.Add(scraped);
                }
            }));
            OnPropertyChanged(nameof(AllUrlData));
            RefreshCheapestData();
        }
        catch
        {
            IsLoaded = false;
        }
    }

    public async Task ChangePartData(string newName, IEnumerable<string> newUrls)
    {
        var newUrlsArray = newUrls.ToArray();
        
        Part.Name = newName;
        OnPropertyChanged(nameof(Name));
        Part.Urls = newUrlsArray;
        _ = Saver.Instance.SaveAsync();

        //Removes all the data from _data that do not exist in newUrls
        _data.RemoveAll(data => !newUrlsArray.Contains(data.Url));
        //Loads all the newUrls that weren't in _data before (updates the cheapest inside)
        await LoadDataAsync(newUrlsArray.Where(newUrl => _data.All(data => data.Url != newUrl)));
    }

    private bool _affordable;
    public bool Affordable 
    {
        get => _affordable;
        set => SetValue(ref _affordable, value);
    }

    public override string ToString() => Name;
}