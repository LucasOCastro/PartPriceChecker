using PriceChecker2.Saving;
using PriceChecker2.UrlScraping;
using System.Diagnostics.CodeAnalysis;

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
    public PartInfo(Part part)
    {
        Part = part;
        LoadDataAsync(part.Urls);
    }

    private void RefreshCheapestData()
    {
        _cheapestData = AllUrlData.Where(data => data.IsValid).MinBy(data => data.Price);
        OnPropertyChanged(nameof(IsValid));
        OnPropertyChanged(nameof(AllUrlData));
        OnPropertyChanged(nameof(LowestPrice));
        OnPropertyChanged(nameof(LowestPriceStoreIconUri));
        OnPropertyChanged(nameof(LowestPriceDomainName));
        OnPropertyChanged(nameof(PriceString));
    }

    private async Task LoadDataAsync(IEnumerable<string> urls)
    {
        IsLoaded = false;
        foreach (var url in urls)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)) continue;
            _data.Add(await UrlScraper.Instance.ScrapeAsync(uri));
        }
        RefreshCheapestData();
        IsLoaded = true;
    }

    public async Task ChangePartData(string newName, IEnumerable<string> newUrls)
    {
        Part.Name = newName;
        OnPropertyChanged(nameof(Name));
        Part.Urls = newUrls.ToArray();
        Task.Run(Saver.Instance.SaveAsync);

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