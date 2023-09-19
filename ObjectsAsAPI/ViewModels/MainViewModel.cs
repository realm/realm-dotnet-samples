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
    private IQueryable<Order> _orders;

    [ObservableProperty]
    private IQueryable<CreateOrderRequest> _createOrderRequests;

    [ObservableProperty]
    private IQueryable<CancelOrderRequest> _cancelOrderRequests;

    [ObservableProperty]
    private string connectionStatusIcon = "wifi_on.png";

    public MainViewModel()
    {
        _realm = RealmService.GetMainThreadRealm();

        // New objects will be at the top of the lists
        _orders = _realm.All<Order>().OrderByDescending( o => o.Content!.CreatedAt);
        _createOrderRequests = _realm.All<CreateOrderRequest>().OrderByDescending(r => r.CreatedAt);
        _cancelOrderRequests = _realm.All<CancelOrderRequest>().OrderByDescending(r => r.CreatedAt);
    }

    [RelayCommand]
    public async Task AddCreateOrderRequest()
    {
        var requestPayload = new CreateOrderPayload
        {
            Content = new OrderContent(),
        };

        var request = new CreateOrderRequest
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
    public async Task OpenOrder(Order order)
    {
        await GoToOrder(order);
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
    public async Task OpenRequest(IRealmObject request)
    {
        if (request is CreateOrderRequest crr)
        {
            await GoToCreateOrderRequest(crr);
        }
        else if (request is CancelOrderRequest cor)
        {
            await GoToCancelOrderRequest(cor);
        }
    }

    [RelayCommand]
    public void DeleteRequest(IRealmObject request)
    {
        _realm.Write(() =>
        {
            _realm.Remove(request);
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

    [RelayCommand]
    public async Task Logout()
    {
        IsBusy = true;
        await RealmService.LogoutAsync();
        IsBusy = false;

        // This is the simplest way to avoid reusing pages after logout
        Application.Current!.MainPage = new AppShell();
    }

    private async Task GoToOrder(Order order)
    {
        var navigationParameter = new Dictionary<string, object>
        {
            { "Order", order },
        };
        await Shell.Current.GoToAsync($"order", navigationParameter);
    }

    private async Task GoToCreateOrderRequest(CreateOrderRequest request)
    {
        var navigationParameter = new Dictionary<string, object>
        {
            { "Request", request },
        };
        await Shell.Current.GoToAsync($"createOrder", navigationParameter);
    }

    private async Task GoToCancelOrderRequest(CancelOrderRequest request)
    {
        var navigationParameter = new Dictionary<string, object>
        {
            { "Request", request },
        };
        await Shell.Current.GoToAsync($"cancelOrder", navigationParameter);
    }
}

