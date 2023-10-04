using AtlasSearch.Views;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace AtlasSearch;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMauiMaps();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}