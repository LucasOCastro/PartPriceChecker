using PriceChecker2.Parts;

namespace PriceChecker2.Pages;

public partial class PartTrackerPage
{
    public PartTrackerPage()
	{
		InitializeComponent();
        Refresh();
        
        PartDatabase.Instance.OnPartRegistered += _ => Refresh();
        PartDatabase.Instance.OnPartUnregistered += _ => Refresh();
    }

    private void Refresh() => PartsCollectionView.ItemsSource = GetSortedParts();

    private static IEnumerable<PartInfo> GetSortedParts() => PartDatabase.Instance.Parts.OrderBy(p => p.Name);
}