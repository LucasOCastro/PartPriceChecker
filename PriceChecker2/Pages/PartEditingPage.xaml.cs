using PriceChecker2.Parts;

namespace PriceChecker2.Pages;

public partial class PartEditingPage : ContentPage, IQueryAttributable
{
	public PartInfo? Part { get; private set; }

	public PartEditingPage()
	{
		InitializeComponent();
	}

	private async void SaveAsync()
	{
		IsBusy = true;
		await Part.ChangePartData(_partEditor.FormatedName, _partEditor.ValidatedUrls);
		IsBusy = false;
		Return();
	}
	private void SaveButton_Pressed(object sender, EventArgs e) => SaveAsync();


    private void Return() => ShellNavigator.Instance.BackAsync();
	private void BackButton_Pressed(object? sender, EventArgs e) => Return();
    protected override bool OnBackButtonPressed()
    {
        Return();
        return true;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!query.TryGetValue(nameof(Part), out var obj) || obj is not PartInfo part) return;

        Part = part;
        _partEditor.Name = part.Name;
		_partEditor.Urls.Clear();
        foreach (var url in part.Part.Urls)
            _partEditor.Urls.Add(url);
    }
}