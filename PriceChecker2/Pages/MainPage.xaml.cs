﻿using PriceChecker2.Parts;

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
            UpdatePercentage();
        }
    }

    private bool TryGetMoneyValue(out double money)
    {
        string moneyString = MoneyString;
        if (string.IsNullOrWhiteSpace(moneyString)) moneyString = "0";
        return double.TryParse(moneyString, out money);
    }

    private string _percentageString;
    public string PercentageString
    {
        get => _percentageString;
        set
        {
            _percentageString = value;
            OnPropertyChanged(nameof(PercentageString));
        }
    }



    public MainPage()
    {
        InitializeComponent();
        LoadPartInfoAsync();
        Build.PropertyChanged += (s, e) => {
            if (e.PropertyName == nameof(Build.TotalValidPrice))
                UpdatePercentage();
        };
    }

    private async Task LoadPartInfoAsync()
    {
        IsBusy = true;
        await AsyncUtils.WaitUntil(() => PartDatabase.Instance.AllLoaded);
        CalculateAffordables();
        IsBusy = false;
    }

    private void UpdatePercentage()
    {
        if (!TryGetMoneyValue(out double money)) return;

        if (Build.TotalValidPrice == 0)
            PercentageString = "100%";
        else
        {
            int percentage = (int)(money * 100f / Build.TotalValidPrice);
            PercentageString = percentage.ToString() + '%';
        }
    }

    private void CalculateAffordables()
    {
        if (!TryGetMoneyValue(out double money)) return;

        foreach (var part in Build.BuildParts)
        {
            if (!part.IsValid)
            {
                part.Affordable = false;
                continue;
            }

            part.Affordable = money >= part.LowestPrice;
            money -= part.LowestPrice;
            if (money < 0) money = 0;
        }
    }
}