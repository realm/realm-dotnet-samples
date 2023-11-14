using CommunityToolkit.Mvvm.ComponentModel;
using IntelligentCache.Models;
using IntelligentCache.Services;
using Realms;

namespace IntelligentCache.ViewModels;

[QueryProperty("Cuisine", nameof(Cuisine))]
public partial class RestaurantsViewModel : BaseViewModel
{
    [ObservableProperty]
    private CuisineType _cuisine;

    [ObservableProperty]
    private IEnumerable<Restaurant> _restaurants = null!;

    private Realm _realm;

    public RestaurantsViewModel()
    {
        _realm = RealmService.GetMainThreadRealm();
    }

    partial void OnCuisineChanging(CuisineType oldValue, CuisineType newValue)
    {
        _realm.Write(() =>
        {
            _realm.Add(new RestaurantsRequest(newValue));
        });

        Restaurants = _realm.All<Restaurant>().Filter($"cuisine == $0", newValue.ToString());
    }
}

