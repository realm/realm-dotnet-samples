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
            Status = RequestStatus.Draft,
            Payload = requestPayload,
        };

        realm.Write(() =>
        {
            realm.Add(request);
        });

        //TODO Is this the way we want to go?
        var shouldCancel = await DialogService.ShowYesNoAlertAsync("Cancel order",
            "Are you sure you want to cancel the current order?");

        realm.Write(() =>
        {
            if (shouldCancel)
            {
                request.Status = RequestStatus.Pending;
            }
            else
            {
                realm.Remove(request);
            }
        });

        await Shell.Current.GoToAsync("..");
    }
}

