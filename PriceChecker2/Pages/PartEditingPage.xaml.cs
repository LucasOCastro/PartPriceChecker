using PriceChecker2.Parts;

namespace PriceChecker2.Pages;

public partial class PartEditingPage : ContentPage
{
	private readonly Part _part;
	
	public PartEditingPage(Part part)
	{
        InitializeComponent();
		_part = part;

		_partEditor.Name = _part.Name;
		foreach (var url in part.Urls)
			_partEditor.Urls.Add(url);
	}

	private async void SaveAsync()
	{
		IsBusy = true;
		await PartDatabase.Instance.SaveChangesAsync();
		IsBusy = false;
	}
	private void SaveButton_Pressed(object sender, EventArgs e)
	{
		_part.Name = _partEditor.Name;
		_part.Urls = _partEditor.ValidatedUrls.ToArray();
		SaveAsync();
	}


	private async Task DeleteAsync()
	{
		IsBusy = true;
		await PartDatabase.Instance.UnregisterAsync(_part);
		IsBusy = false;
	}
	private void DeleteButton_Pressed(object sender, EventArgs e) => DeleteAsync();
}