using PriceChecker2.Parts;

namespace PriceChecker2.Pages;

public partial class PartAddPage
{
	public PartAddPage()
	{
		InitializeComponent();
		BindingContext = this;
		PartEditor.Urls.Add("");
	}

	private async Task SaveAsync(Part part)
	{
        IsBusy = true;
        await PartDatabase.Instance.RegisterAsync(part);
        IsBusy = false;
		PartEditor.ClearInputs();
    }

    private void SaveButton_Pressed(object sender, EventArgs e)
	{
		Part part = new()
		{
			Name = PartEditor.FormatedName,
			Urls = PartEditor.ValidatedUrls.ToArray()
		};
        _ = SaveAsync(part);
	}
}