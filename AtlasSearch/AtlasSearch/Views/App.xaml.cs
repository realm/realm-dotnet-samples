using AtlasSearch.Converters;

namespace AtlasSearch.Views;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        Resources.Add("HighlightFormattedStringConverter", new HighlightFormattedStringConverter());

        MainPage = new AppShell();

        Routing.RegisterRoute("autocomplete", typeof(AutocompletePage));
        Routing.RegisterRoute("compound", typeof(CompoundPage));
    }
}