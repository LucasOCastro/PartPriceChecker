using HtmlAgilityPack;

namespace PriceChecker2.UrlScraping;

public class UrlScraper : Singleton<UrlScraper>
{
    private readonly HttpClient _client = new();

    private static HtmlNode? GetPriceNode(IEnumerable<HtmlNode> nodes, string website) 
        => website switch
        {
            "kabum" => nodes.FirstOrDefault(n => n.GetClasses().Any(c => c == "finalPrice")),
            "pichau" => nodes.FirstOrDefault(n => n.InnerText == "à vista").NextSiblingElement(),
            "amazon" => nodes.FirstOrDefault(n => n.HasClass("a-price-whole")),
            "mercadolivre" => nodes.FirstOrDefault(n => n.HasClass("andes-money-amount__fraction")),
            _ => null,
        };

    private static string? ProcessPriceString(string priceString, string website)
        => website switch
        {
            "kabum" => priceString.Replace(".", "").Replace(",", "."),
            "pichau" => priceString.Replace(",", ""),
            "amazon" => priceString.Replace(".", ""),
            "mercadolivre" => priceString.Replace(".", ""),
            _ => null
        };

    private static string GetDomainName(Uri uri) => uri.Host.ToLower()
        .Replace("www", "")
        .Replace("com", "")
        .Replace("br", "")
        .Replace("produto", "")
        .Replace(".", "");

    private bool IsIconNode(HtmlNode node)
    {
        if (node.Name != "link" || !node.Attributes.Contains("rel")) return false;
        string rel = node.Attributes["rel"].Value;
        return rel == "icon" || rel == "shortcut icon";
    }

    public async Task<UrlScrapedData?> ScrapeAsync(Uri url)
    {
        if (!url.IsWellFormedOriginalString()) return null;

        string html = await _client.GetStringAsync(url);
        
        HtmlDocument doc = new();
        doc.LoadHtml(html);
        string iconUri = doc.DocumentNode.Descendants().FirstOrDefault(IsIconNode)?.Attributes["href"]?.Value ?? "";// "http://www.google.com/s2/favicons?domain=" + url.Host;
        if (Uri.IsWellFormedUriString(iconUri, UriKind.Relative)) iconUri = "https://" + url.Host + iconUri;

        string domain = GetDomainName(url);
        var priceNode = GetPriceNode(doc.DocumentNode.Descendants(), domain);
        if (priceNode == null) return null;

        string priceString = ProcessPriceString(priceNode.InnerText, domain) ?? "0";
        double price = double.Parse(priceString.Replace(" ", "").Replace("r$", "", StringComparison.OrdinalIgnoreCase).Trim());

        return new()
        {
            Url = url.ToString(),
            WebsiteIconUri = iconUri,
            Price = price,
            DomainName = domain
        };
    }

    ~UrlScraper()
    {
        _client.Dispose();
    }
}
