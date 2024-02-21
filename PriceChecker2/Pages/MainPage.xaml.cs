using PriceChecker2.Parts;

namespace PriceChecker2.Pages;

public partial class MainPage : ContentPage
{
    public Build Build { get; } = new();

    private string _moneyString = "0";
    public string MoneyString
    {
        get => _moneyString;
        set
        {
            _moneyString = value;
            OnPropertyChanged(nameof(MoneyString));
            CalculateAffordables();
        }
    }

    public MainPage()
    {
        InitializeComponent();
        LoadPartInfoAsync();
    }

    private async Task LoadPartInfoAsync()
    {
        IsBusy = true;
        await AsyncUtils.WaitUntil(() => PartDatabase.Instance.AllLoaded);
        CalculateAffordables();
        IsBusy = false;
    }

    private void CalculateAffordables()
    {
        if (!double.TryParse(MoneyString, out double money)) return;
        foreach (var part in Build.BuildParts.Where(bp => bp.IsValid))
        {
            part.Affordable = money >= part.LowestPrice;
            money -= part.LowestPrice;
            if (money < 0) money = 0;
        }
    }
}