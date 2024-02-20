using HtmlAgilityPack;

namespace PriceChecker2.UrlScraping;

public class UrlScraper : Singleton<UrlScraper>
{
    private readonly HttpClient _client = new();
    private static readonly char[] anyOf = new char[]{',', '.'};

    private static HtmlNode? GetPriceNode(IEnumerable<HtmlNode> nodes, string website) 
        => website switch
        {
            "kabum" => nodes.FirstOrDefault(n => n.GetClasses().Any(c => c == "finalPrice")),
            "pichau" => nodes.FirstOrDefault(n => n.Id == "valVista"),
            "amazon" => nodes.FirstOrDefault(n => n.HasClass("a-price-whole")),
            "mercadolivre" => nodes.FirstOrDefault(n => n.HasClass("andes-money-amount__fraction")),
            _ => null,
        };

    private static string GetDomainName(Uri uri) => uri.Host.ToLower()
        .Replace("www", "")
        .Replace("com", "")
        .Replace("br", "")
        .Replace("produto", "")
        .Replace(".", "");

    //Must always use . for decimals and nothing for thousands
    private static string ProcessPriceString(string price)
    {
        price = price.Replace(" ", "")
            .Replace("r$", "", StringComparison.OrdinalIgnoreCase);

        int firstIndex = price.IndexOfAny(anyOf);
        if (firstIndex == -1) return price;
        int lastIndex = price.LastIndexOfAny(anyOf);

        //Ex: 7.999,99 or 7,999.99, must become 7999.99
        if (firstIndex != lastIndex)
            price = string.Concat(price.AsSpan(0, firstIndex), price.AsSpan(firstIndex + 1)); //Remove the first symbol
        //ELSE Ex: 799.99 or 799,99, must become 799.99
        return price.Replace(',', '.');
    }

    public async Task<UrlScrapedData?> ScrapeAsync(Uri url)
    {
        string html = await _client.GetStringAsync(url);
        
        HtmlDocument doc = new();
        doc.LoadHtml(html);
        string iconUri = "http://www.google.com/s2/favicons?domain=" + url.Host;


        var priceNode = GetPriceNode(doc.DocumentNode.Descendants(), GetDomainName(url));
        if (priceNode == null) return null;

        string priceString = ProcessPriceString(priceNode.InnerText);
        double price = double.Parse(priceString.Trim());

        return new()
        {
            WebsiteIconUri = iconUri,
            Price = price
        };
    }

    ~UrlScraper()
    {
        _client.Dispose();
    }
}
