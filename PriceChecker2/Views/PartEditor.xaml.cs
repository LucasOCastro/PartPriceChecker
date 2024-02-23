using System.Collections.ObjectModel;

namespace PriceChecker2.Views;

public partial class PartEditor : ContentView
{
	private string _name = "";
	public string Name
	{
		get => _name;
		set 
		{
			_name = value;
			OnPropertyChanged(nameof(Name));
		}
	}

    public string FormatedName => Name.Trim();

	public ObservableCollection<Observable<string>> Urls { get; } = new();

    public IEnumerable<string> ValidatedUrls
        => Urls.Select(u => u.Value.Trim()).Where(u => Uri.IsWellFormedUriString(u, UriKind.Absolute));

    public PartEditor()
	{
        InitializeComponent();
    }

    public void ClearInputs()
    {
        _partNameEntry.Text = "";
        Urls.Clear();
    }

    private void AddUrl_Pressed(object sender, EventArgs e) => Urls.Add("");

    private void RemoveItem_Pressed(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Observable<string> url)
            Urls.Remove(url);
    }
}