using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntelligentCache.Models;
using IntelligentCache.Services;

namespace IntelligentCache.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    [ObservableProperty]
    private IEnumerable<CuisineType> _cuisines;

    public MainViewModel()
    {
        _cuisines = (CuisineType[])Enum.GetValues(typeof(CuisineType));
    }

    [RelayCommand]
    public async Task GoToRestaurants(CuisineType cuisine)
    {
        var navigationParameter = new Dictionary<string, object>
        {
            { "Cuisine", cuisine },
        };
        await Shell.Current.GoToAsync($"restaurants", navigationParameter);
    }

    [RelayCommand]
    public async Task Logout()
    {
        IsBusy = true;
        await RealmService.LogoutAsync();
        IsBusy = false;

        // This is the simplest way to avoid reusing pages after logout
        Application.Current!.MainPage = new AppShell();
    }
}

