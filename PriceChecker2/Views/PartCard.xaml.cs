using PriceChecker2.Pages;
using PriceChecker2.Parts;

namespace PriceChecker2.Views;

public partial class PartCard
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

    public bool LinkToViewerOnPress
	{
		get => (bool)GetValue(LinkToViewerOnPressProperty);
		set => SetValue(LinkToViewerOnPressProperty, value);

	}
	public static readonly BindableProperty LinkToViewerOnPressProperty =
		BindableProperty.Create(nameof(LinkToViewerOnPress), typeof(bool), typeof(PartCard), defaultValue: true);

    public PartCard()
	{
		InitializeComponent();
	}

	protected override void OnPropertyChanged(string propertyName = null!)
	{
		base.OnPropertyChanged(propertyName);

		if (propertyName == nameof(Part) && Part is { IsLoaded: false })
			Part.BeginLoading();
	}

	private void Frame_Tapped(object? sender, TappedEventArgs args)
	{
		if (!LinkToViewerOnPress || Part == null) return;
		_ = ShellNavigator.Instance.NavigateAsync("//viewer", new Dictionary<string, object?> { { nameof(PartViewingPage.Part), Part } });
	}
}