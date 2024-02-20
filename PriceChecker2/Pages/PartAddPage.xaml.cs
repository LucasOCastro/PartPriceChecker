using System.Collections.ObjectModel;
using PriceChecker2.Parts;

namespace PriceChecker2;

public partial class PartAddPage : ContentPage
{
	private class URL : ObservableViewModel
    {
		private string _value = "";
		public string Value 
		{
			get => _value;
			set => SetValue(ref _value, value);
		}

		public URL() { }

		public static implicit operator string(URL url) => url.Value;
		public static implicit operator URL(string url) => new() { Value = url };
	}

	private readonly ObservableCollection<URL> _urls = new() { "" };
	public PartAddPage()
	{
		InitializeComponent();
		_entryColView.ItemsSource = _urls;
		BindingContext = this;
	}

	private void ClearInputs()
	{
        _partNameEntry.Text = "";
        _urls.Clear();
    }

    private void AddUrl_Pressed(object sender, EventArgs e) => _urls.Add("");

    private void RemoveItem_Pressed(object sender, EventArgs e)
	{
        if (sender is Button button && button.CommandParameter is URL url)
			_urls.Remove(url);
    }

	private async Task SaveAsync(Part part)
	{
        IsBusy = true;
        await PartDatabase.Instance.RegisterAsync(part);
        IsBusy = false;
		ClearInputs();
    }

    private void SaveButton_Pressed(object sender, EventArgs e)
	{
		Part part = new()
		{
			Name = _partNameEntry.Text.Trim(),
			Urls = _urls.Select(u => u.Value.Trim()).Where(u => Uri.IsWellFormedUriString(u, UriKind.Absolute)).ToArray()
		};
        SaveAsync(part);
	}
}