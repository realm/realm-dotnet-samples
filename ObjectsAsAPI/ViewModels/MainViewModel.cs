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
    private List<IQueryable<IRealmObject>> _requests;

    [ObservableProperty]
    private string connectionStatusIcon = "wifi_on.png";

    public MainViewModel()
    {
        _realm = RealmService.GetMainThreadRealm();

        // New objects will be on top
        _orders = _realm.All<Order>().OrderByDescending( o => o.Content!.CreatedAt);
        var createOrderRequests = _realm.All<CreateOrderRequest>().OrderByDescending(r => r.CreatedAt);
        var cancelOrderRequests = _realm.All<CancelOrderRequest>().OrderByDescending(r => r.CreatedAt);

        _requests = new List<IQueryable<IRealmObject>>
        {
            createOrderRequests,
            cancelOrderRequests
        };
    }

    [RelayCommand]
    public async Task CreateOrderRequest()
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
    public async Task OpeRequest(IRealmObject request)
    {
        if (request?.ObjectSchema?.Name == nameof(CreateOrderRequest))
        {
            await GoToCreateOrderRequest((CreateOrderRequest)request);
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

    public async Task GoToOrder(Order order)
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
        await Shell.Current.GoToAsync($"createModifyOrder", navigationParameter);
    }
}

