using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ObjectsAsAPI.Models;
using ObjectsAsAPI.Services;
using Realms;

namespace ObjectsAsAPI.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private bool isOnline = true;

    private Realm _realm;

    [ObservableProperty]
    private IEnumerable<Order> _orders;

    [ObservableProperty]
    private IEnumerable<AtlasRequest> _requests;

    [ObservableProperty]
    private string connectionStatusIcon = "wifi_on.png";

    public MainViewModel()
    {
        _realm = RealmService.GetMainThreadRealm();

        // New objects will be on top
        _orders = _realm.All<Order>().OrderByDescending( o => o.Content!.CreatedAt);
        _requests = _realm.All<AtlasRequest>().OrderByDescending(r => r.CreatedAt);
    }

    [RelayCommand]
    public async Task CreateOrderRequest()
    {
        var requestPayload = new CreateOrderPayload
        {
            Content = new OrderContent(),
        };

        var request = new AtlasRequest
        {
            Status = RequestStatus.Draft,
            Payload = requestPayload,
        };

        _realm.Write(() =>
        {
            _realm.Add(request);
        });

        await GoToCreateOrderRequest(request);
    }

    [RelayCommand]
    public async Task OpenRequest(AtlasRequest request)
    {
        //TODO Here I should differentiate between the requests
        await GoToCreateOrderRequest(request);
    }

    [RelayCommand]
    public async Task OpenOrder(Order order)
    {
        await GoToOrder(order);
    }

    [RelayCommand]
    public void DeleteRequest(AtlasRequest request)
    {
        _realm.Write(() =>
        {
            _realm.Remove(request);
        });
    }

    [RelayCommand]
    public void DeleteOrder(Order order)
    {
        _realm.Write(() =>
        {
            _realm.Remove(order);
        });
    }

    [RelayCommand]
    public void ChangeConnectionStatus()
    {
        isOnline = !isOnline;

        if (isOnline)
        {
            _realm.SyncSession.Start();
        }
        else
        {
            _realm.SyncSession.Stop();
        }

        ConnectionStatusIcon = isOnline ? "wifi_on.png" : "wifi_off.png";
    }

    public async Task GoToOrder(Order order)
    {
        var navigationParameter = new Dictionary<string, object>
        {
            { "Order", order },
        };
        await Shell.Current.GoToAsync($"order", navigationParameter);
    }

    private async Task GoToCreateOrderRequest(AtlasRequest request)
    {
        var navigationParameter = new Dictionary<string, object>
        {
            { "Request", request },
        };
        await Shell.Current.GoToAsync($"createModifyOrder", navigationParameter);
    }
}

