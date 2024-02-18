using System.Collections.ObjectModel;

namespace PriceChecker2;

public partial class PartAddPage : ContentPage
{
	private class URL : ObservableModelView
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

	private readonly ObservableCollection<URL> _urls = new();
	public PartAddPage()
	{
		InitializeComponent();
		_entryColView.ItemsSource = _urls;
		BindingContext = this;
	}

    private void AddUrl_Pressed(object sender, EventArgs e) => _urls.Add("");

    private void RemoveItem_Pressed(object sender, EventArgs e)
	{
        if (sender is Button button && button.CommandParameter is URL url)
			_urls.Remove(url);
    }

    private void SaveButton_Pressed(object sender, EventArgs e)
	{
		Part part = new()
		{
			Name = _partNameEntry.Text,
			Urls = _urls.Select(u => u.Value).ToArray()
		};
		//TODO some loading window until finishes saving
		PartDatabase.Instance.RegisterAsync(part);

		_partNameEntry.Text = "";
		_urls.Clear();
	}
}