using Acr.UserDialogs;
using AtlasSearch.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AtlasSearch.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [RelayCommand]
    private static async Task InitializeSearch()
    {
        if (SearchService.IsInitialized)
        {
            return;
        }

        try
        {
            using var loadingDialog = UserDialogs.Instance.Loading("Initializing Realm service");

            loadingDialog.Show();
            await SearchService.Initialize();
        }
        catch (Exception ex)
        {
            await UserDialogs.Instance.AlertAsync($"An error occurred while initializing the Realm app: {ex.Message}", "Initialization failed");

            Application.Current!.Quit();
        }
    }

    [RelayCommand]
    private static Task Navigate(string route) => Shell.Current.GoToAsync(route);
}
