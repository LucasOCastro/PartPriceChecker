using PriceChecker2.Parts;

namespace PriceChecker2.Pages;

public partial class PartEditingPage : IQueryAttributable
{
	public PartInfo? Part { get; private set; }

	public PartEditingPage()
	{
		InitializeComponent();
	}

	private async Task SaveAsync()
	{
		if (Part is null) return;
		
		IsBusy = true;
		await Part.ChangePartData(PartEditor.FormatedName, PartEditor.ValidatedUrls);
		IsBusy = false;
		NavigateBack();
	}
	private void SaveButton_Pressed(object sender, EventArgs e) => _ = SaveAsync();

	
	private void BackButton_Pressed(object? sender, EventArgs e) => NavigateBack();
    protected override bool OnBackButtonPressed()
    {
        NavigateBack();
        return true;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!query.TryGetValue(nameof(Part), out var obj) || obj is not PartInfo part) return;

        Part = part;
        PartEditor.Name = part.Name;
		PartEditor.Urls.Clear();
        foreach (var url in part.Part.Urls)
            PartEditor.Urls.Add(url);
    }
    
    private static void NavigateBack() => _ = ShellNavigator.Instance.BackAsync();
}