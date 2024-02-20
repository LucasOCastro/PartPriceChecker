using PriceChecker2.Parts;

namespace PriceChecker2.Pages;

public partial class PartViewingPage : ContentPage, IQueryAttributable
{
    public PartInfo? Part
    {
        get => GetValue(PartProperty) as PartInfo;
        set
        {
            SetValue(PartProperty, value);
            BindingContext = Part;
        }
    }
    public readonly static BindableProperty PartProperty =
        BindableProperty.Create(nameof(Part), typeof(PartInfo), typeof(PartViewingPage));

    public PartViewingPage()
    {
        InitializeComponent();
    }

    private void EditButton_Pressed(object sender, EventArgs e)
    {
        ShellNavigator.Instance.NavigateAsync("//editor", new() { { nameof(PartEditingPage.Part), Part?.Part } });
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!query.TryGetValue(nameof(Part), out var obj) || obj is not PartInfo part) return;
        Part = part;
    }
}