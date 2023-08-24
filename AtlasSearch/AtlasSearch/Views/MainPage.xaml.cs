using AtlasSearch.ViewModels;

namespace AtlasSearch.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _ = ((MainViewModel)BindingContext).InitializeSearch();
    }
}
