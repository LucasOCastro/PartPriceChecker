namespace PriceChecker2.UrlScraping;

public class UrlScrapedData
{
    public string Url { get; set; } = "";
    public string WebsiteIconUri { get; set; } = "";
    public double Price { get; set; } = -1;
    public string PriceString => Price > 0 ? string.Format("{0:C}", Price) : "INVALID";
}
