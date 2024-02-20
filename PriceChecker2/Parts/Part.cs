namespace PriceChecker2.Parts;

public class Part
{
    public string Name { get; set; } = "New Part";
    public string[] Urls { get; set; } = Array.Empty<string>();
    public bool IsBuildPart { get; set; } = false;

    public override string ToString() => Name;
}
