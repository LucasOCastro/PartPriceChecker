using PriceChecker2.Parts;
using System.Collections.Specialized;

namespace PriceChecker2.Pages;

public partial class PartTrackerPage
{
    private static IEnumerable<PartInfo> SortedParts => PartDatabase.Instance.Parts.OrderBy(p => p.Name);


    public PartTrackerPage()
	{
		InitializeComponent();
        foreach (var part in PartDatabase.Instance.Parts) 
            part.BeginLoading();
        PartsCollectionView.ItemsSource = SortedParts;

        ((INotifyCollectionChanged)PartDatabase.Instance.Parts).CollectionChanged += (_, _) => OnPropertyChanged(nameof(SortedParts));
    }
}