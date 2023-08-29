using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ObjectsAsAPI.Models;
using ObjectsAsAPI.Services;

namespace ObjectsAsAPI.ViewModels;

[QueryProperty("Order", nameof(Order))]
public partial class OrderViewModel : BaseViewModel
{
    [ObservableProperty]
    private Order _order = null!;

    [RelayCommand]
    public async Task CancelOrder()
    {
        var realm = RealmService.GetMainThreadRealm();

        var requestPayload = new CancelOrderPayload
        {
            OrderId = Order.Id,
        };

        var request = new AtlasRequest
        {
            Status = RequestStatus.Pending,
            Payload = requestPayload,
        };

        realm.Write(() =>
        {
            realm.Add(request);
        });

        await Shell.Current.GoToAsync("..");
    }
}

