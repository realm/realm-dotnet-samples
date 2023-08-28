using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ObjectsAsAPI.Models;
using ObjectsAsAPI.Services;
using Realms;

namespace ObjectsAsAPI.ViewModels;

[QueryProperty("Request", "Request")]
public partial class CreateOrderViewModel : BaseViewModel
{
    [ObservableProperty]
    private OrderContent _orderContent = null!;

    [ObservableProperty]
    private AtlasRequest _request = null!;

    [ObservableProperty]
    private bool _isDraft;

    [ObservableProperty]
    private bool _isNotDraft; //TODO Need to remove this

    private Realm _realm;

    public CreateOrderViewModel()
    {
        _realm = RealmService.GetMainThreadRealm();
    }

    [RelayCommand]
    public void DeleteItem(OrderItem item)
    {
        _realm.Write(() =>
        {
            OrderContent.Items.Remove(item);
        });
    }

    [RelayCommand]
    public void AddItem()
    {
        _realm.Write(() =>
        {
            OrderContent.Items.Add(new OrderItem());
        });
    }

    [RelayCommand]
    public async Task Confirm()
    {
        _realm.Write(() =>
        {
            Request.Status = RequestStatus.Pending;
        });

        await Close();
    }

    [RelayCommand]
    public async Task Delete()
    {
        _realm.Write(() =>
        {
            _realm.Remove(Request);
        });

        await Close();
    }

    partial void OnRequestChanged(AtlasRequest value)
    {
        if (value == null)
        {
            return;
        }

        OrderContent = value.Payload.As<CreateOrderPayload>().Content!;
        IsDraft = value.Status == RequestStatus.Draft;
        IsNotDraft = value.Status != RequestStatus.Draft;
    }

    private async Task Close()
    {
        await Shell.Current.GoToAsync("..");
    }
}

