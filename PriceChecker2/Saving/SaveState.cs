using PriceChecker2.Parts;
using System.Text.Json;

namespace PriceChecker2.Saving;

public class SaveState
{
    public double Money { get; set; } = 0;
    public List<Part> Parts { get; set; } = new();
}
