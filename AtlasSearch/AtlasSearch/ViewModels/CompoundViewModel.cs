using Acr.UserDialogs;
using AtlasSearch.Models;
using AtlasSearch.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Maps;

namespace AtlasSearch.ViewModels;

public partial class CompoundViewModel : SearchableViewModelBase<Listing, (string Query, MapSpan MapSpan)>
{
    [ObservableProperty]
    private MapSpan mapSpan = MapSpan.FromCenterAndRadius(new(40.7831, -73.9712), Distance.FromKilometers(10)); // Manhattan

    protected override async Task Search((string Query, MapSpan MapSpan) args)
    {
        if (string.IsNullOrEmpty(args.Query))
        {
            return;
        }

        try
        {
            // * 0.75 because the circle radius is 300 vs the map radius being 400.
            var distance = args.MapSpan.Radius.Meters * 0.75;
            var results = await SearchService.Compound(args.Query, args.MapSpan.Center, distance);
            if (args.Query == SearchQuery && args.MapSpan == MapSpan)
            {
                Results = results;
            }
        }
        catch (Exception ex)
        {
            UserDialogs.Instance.Alert($"An error occurred while executing search: {ex}", "Failed to execute search");
        }
    }

    partial void OnMapSpanChanged(MapSpan value)
    {
        TriggerSearch();
    }

    protected override void TriggerSearch()
    {
        _searchDebouncer((SearchQuery, MapSpan));
    }
}
