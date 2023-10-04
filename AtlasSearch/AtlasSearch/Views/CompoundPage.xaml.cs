using AtlasSearch.ViewModels;

using Map = Microsoft.Maui.Controls.Maps.Map;

namespace AtlasSearch.Views;

public partial class CompoundPage : ContentPage
{
    public CompoundPage()
    {
        InitializeComponent();

        var vm = (CompoundViewModel)BindingContext;

        SearchMap.MoveToRegion(vm.MapSpan);

        SearchMap.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(Map.VisibleRegion) && SearchMap.VisibleRegion != null)
            {
                vm.MapSpan = SearchMap.VisibleRegion;
            }
        };
    }
}