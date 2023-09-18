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
        var shouldCancel = await DialogService.ShowYesNoAlertAsync("Cancel order",
            "Are you sure you want to cancel the current order?");

        if (!shouldCancel)
        {
            return;
        }

        var realm = RealmService.GetMainThreadRealm();

        var request = new CancelOrderRequest
        {
            Status = RequestStatus.Pending,
            Payload = new CancelOrderPayload
            {
                OrderId = Order.Id,
            },
        };

        realm.Write(() =>
        {
            realm.Add(request);
        });

        await Shell.Current.GoToAsync("..");
    }
}

