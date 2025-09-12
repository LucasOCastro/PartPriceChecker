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
            "terabyteshop" => nodes.FirstOrDefault(n => n.Id == "valVista"),
            "amazon" => nodes.FirstOrDefault(n => n.HasClass("a-price-whole")),
            "mercadolivre" => nodes.FirstOrDefault(n => n.HasClass("andes-money-amount__fraction")),
            _ => null,
        };

    private static string? ProcessPriceString(string priceString, string website)
        => website switch
        {
            "kabum" => priceString.Replace(".", "").Replace(',', '.'),
            "pichau" => priceString.Replace(",", ""),
            "terabyteshop" => priceString.Replace(".", "").Replace(',', '.'),
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

    private static HtmlNode? GetIconNode(IEnumerable<HtmlNode> nodes)
    {
        foreach (var node in nodes)
        {
            // Apple
            if (IsIcon(node, "apple-touch-icon"))
                return node;
            
            if (IsIcon(node, "icon", "shortcut icon"))
                return node;
        }
        
        return null;

        static bool IsIcon(HtmlNode node, params string[] types) 
            => node.Name == "link" && node.Attributes.Contains("rel") && types.Contains(node.Attributes["rel"].Value);
    }

    public async Task<UrlScrapedData?> ScrapeAsync(Uri url)
    {
        if (!url.IsWellFormedOriginalString()) return null;

        string html = await _client.GetStringAsync(url);
        
        HtmlDocument doc = new();
        doc.LoadHtml(html);
        string iconUri = GetIconNode(doc.DocumentNode.Descendants())?.Attributes["href"]?.Value ?? "";
        if (Uri.IsWellFormedUriString(iconUri, UriKind.Relative)) iconUri = "https://" + url.Host + iconUri;

        string domain = GetDomainName(url);

        double price = -1;
        var priceNode = GetPriceNode(doc.DocumentNode.Descendants(), domain);
        if (priceNode != null)
        {
            string priceString = ProcessPriceString(priceNode.InnerText, domain) ?? "0";
            priceString = priceString.Replace(" ", "").Replace("r$", "", StringComparison.OrdinalIgnoreCase).Trim();
            if (double.TryParse(priceString, out double priceValue)) price = priceValue;
        }

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
