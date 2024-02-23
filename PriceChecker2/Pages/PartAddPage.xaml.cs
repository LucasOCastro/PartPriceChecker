using PriceChecker2.Parts;

namespace PriceChecker2.Pages;

public partial class PartAddPage : ContentPage
{
	public PartAddPage()
	{
		InitializeComponent();
		BindingContext = this;
		_partEditor.Urls.Add("");
	}

	private async Task SaveAsync(Part part)
	{
        IsBusy = true;
        await PartDatabase.Instance.RegisterAsync(part);
        IsBusy = false;
		_partEditor.ClearInputs();
    }

    private void SaveButton_Pressed(object sender, EventArgs e)
	{
		Part part = new()
		{
			Name = _partEditor.FormatedName,
			Urls = _partEditor.ValidatedUrls.ToArray()
		};
        SaveAsync(part);
	}
}