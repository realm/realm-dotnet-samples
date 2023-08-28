using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ObjectsAsAPI.Models;
using ObjectsAsAPI.Services;
using Realms;

namespace ObjectsAsAPI.ViewModels;

public partial class OrdersViewModel : BaseViewModel
{
    [ObservableProperty]
    private IEnumerable<Order> _orders;

    [ObservableProperty]
    private IEnumerable<AtlasRequest> _requests;

    private bool isOnline = true;  //TODO Order variables

    [ObservableProperty]
    private string connectionStatusIcon = "wifi_on.png";

    private Realm _realm;

    public OrdersViewModel()
    {
        _realm = RealmService.GetMainThreadRealm();

        _orders = _realm.All<Order>();
        _requests = _realm.All<AtlasRequest>();
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

