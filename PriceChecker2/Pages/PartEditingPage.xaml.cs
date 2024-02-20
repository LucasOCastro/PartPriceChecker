using PriceChecker2.Parts;

namespace PriceChecker2.Pages;

public partial class PartEditingPage : ContentPage, IQueryAttributable
{
	public Part? Part { get; private set; }

	public PartEditingPage()
	{
		InitializeComponent();
	}

	private async void SaveAsync()
	{
		IsBusy = true;
		await PartDatabase.Instance.SaveChangesAsync();
		IsBusy = false;
		Return();
	}
	private void SaveButton_Pressed(object sender, EventArgs e)
	{
		Part.Name = _partEditor.Name;
        Part.Urls = _partEditor.ValidatedUrls.ToArray();
		SaveAsync();
	}


	private async Task DeleteAsync()
	{
		IsBusy = true;
		await PartDatabase.Instance.UnregisterAsync(Part);
		IsBusy = false;
		Return();
	}
	private void DeleteButton_Pressed(object sender, EventArgs e) => DeleteAsync();

	private void Return() => Shell.Current.GoToAsync("//main");
	private void BackButton_Pressed(object? sender, EventArgs e) => Return();

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
		if (query[nameof(Part)] is not Part part) return;
		Part = part;
        _partEditor.Name = part.Name;
        foreach (var url in part.Urls)
            _partEditor.Urls.Add(url);
    }
}