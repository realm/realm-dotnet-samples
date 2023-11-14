using IntelligentCache.Views;

namespace IntelligentCache;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("restaurants", typeof(RestaurantsPage));
    }
}

