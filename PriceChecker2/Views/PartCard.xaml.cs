using PriceChecker2.Pages;
using PriceChecker2.Parts;

namespace PriceChecker2.Views;

public partial class PartCard : ContentView
{
	public PartInfo? Part
	{
		get => (PartInfo?)GetValue(PartProperty);
		set 
		{
			SetValue(PartProperty, value);
			BindingContext = Part;
		}
	}

	public static readonly BindableProperty PartProperty =
		BindableProperty.Create(nameof(Part), typeof(PartInfo), typeof(PartCard));

	public PartCard()
	{
		InitializeComponent();
	}

	private void Frame_Tapped(object? sender, TappedEventArgs args)
		//=> Shell.Current.GoToAsync("//editor", new Dictionary<string, object?> { { nameof(PartEditingPage.Part), Part?.Part } });
		=> ShellNavigator.Instance.NavigateAsync("//editor", new Dictionary<string, object?> { { nameof(PartEditingPage.Part), Part?.Part } });
}