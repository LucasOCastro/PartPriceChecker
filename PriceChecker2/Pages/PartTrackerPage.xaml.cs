using PriceChecker2.Parts;
using System.Collections.Specialized;

namespace PriceChecker2.Pages;

public partial class PartTrackerPage : ContentPage
{
    private static IEnumerable<PartInfo> SortedParts => PartDatabase.Instance.Parts.OrderBy(p => p.Name);


    public PartTrackerPage()
	{
		InitializeComponent();
        //LoadPartInfoAsync();
        _partsCollectionView.ItemsSource = SortedParts;

        ((INotifyCollectionChanged)PartDatabase.Instance.Parts).CollectionChanged += (o, e) => OnPropertyChanged(nameof(SortedParts));
    }
    
    /*private async Task LoadPartInfoAsync()
    {
        _partsCollectionView.ItemsSource = SortedParts;
        await AsyncUtils.WaitUntil(() => PartDatabase.Instance.AllLoaded);
    }*/
}