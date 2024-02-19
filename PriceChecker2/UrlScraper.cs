using HtmlAgilityPack;

namespace PriceChecker2.UrlScraping;

public class UrlScrapedData
{
    public string WebsiteIconUri { get; set; } = "";
    public string PriceString { get; set; } = "";
}

public class UrlScraper : Singleton<UrlScraper>
{
    private readonly HttpClient _client = new();

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

    public async Task<UrlScrapedData?> ScrapeAsync(Uri url)
    {
        string html = await _client.GetStringAsync(url);
        

        HtmlDocument doc = new();
        doc.LoadHtml(html);
        string iconUri = doc.DocumentNode.SelectSingleNode("/html/head/link[@rel='shortcut icon' and @href]").Attributes["href"].Value;


        var priceNode = GetPriceNode(doc.DocumentNode.Descendants(), GetDomainName(url));
        if (priceNode == null) return null;

        string priceString = priceNode.InnerText
            .Replace("r$", "", StringComparison.OrdinalIgnoreCase)
            .Replace(" ", "")
            .Replace(',', '.');

        return new()
        {
            WebsiteIconUri = iconUri,
            PriceString = priceString
        };
    }

    ~UrlScraper()
    {
        _client.Dispose();
    }
}
