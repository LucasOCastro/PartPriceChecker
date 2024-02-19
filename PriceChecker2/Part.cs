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

    /*public double LowestPrice => _data.MinBy(data => data.CurrentPrice)?.CurrentPrice ?? -1;
    public string LowestPriceStoreIconUri => _data.MinBy(data => data.CurrentPrice)?.WebsiteIconUri ?? "";
    public string PriceString => string.Format("{0:C}", LowestPrice);*/

    public double LowestPrice { get; private set; } = -1;
    public string LowestPriceStoreIconUri { get; private set; }
    public string PriceString { get; private set; }

    private void SetLowestPrice(UrlScrapedData data)
    {
        double price = 0;
        try
        {
            price = double.Parse(data.PriceString.Trim());
        }
        catch (Exception e)
        {
            ;
        }
        
        if (LowestPrice < 0 || price < LowestPrice)
        {
            LowestPrice = price;
            PriceString = string.Format("{0:C}", LowestPrice);
            LowestPriceStoreIconUri = data.WebsiteIconUri;
        }
    }


    private UrlScrapedData[] _data = Array.Empty<UrlScrapedData>();
    public PartInfo(Part part)
    {
        Name = part.Name;
        Loading = true;
        LoadDataAsync(part.Urls);
    }

    private async Task LoadDataAsync(string[] urls)
    {
        _data = new UrlScrapedData[urls.Length];
        for (int i = 0; i < urls.Length; i++)
        {
            var data = await UrlScraper.Instance.ScrapeAsync(new Uri(urls[i]));
            if (data != null) SetLowestPrice(data);
            _data[i] = data ?? new UrlScrapedData();
        }
        Loading = false;
    }

    public override string ToString() => Name;
}