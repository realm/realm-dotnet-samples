using CommunityToolkit.Mvvm.ComponentModel;
using ObjectsAsAPI.Models;
using ObjectsAsAPI.Services;
using Realms;

namespace ObjectsAsAPI.ViewModels;

public partial class RequestsViewModel : BaseViewModel
{
    [ObservableProperty]
    private IEnumerable<AtlasRequest> _requests;

    [ObservableProperty]
    private string connectionStatusIcon = "wifi_on.png";

    private Realm _realm;

    public RequestsViewModel()
    {
        _realm = RealmService.GetMainThreadRealm();

        // New objects will be on top
        _requests = _realm.All<AtlasRequest>().OrderByDescending(r => r.CreatedAt);
    }
}

