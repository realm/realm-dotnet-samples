using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ObjectsAsAPI.Models;
using ObjectsAsAPI.Services;
using Realms;

namespace ObjectsAsAPI.ViewModels;

public partial class CreateModifyOrderViewModel : BaseViewModel
{
    [ObservableProperty]
    private OrderContent _orderContent;

    private AtlasRequest _request;

    private Realm _realm;

    public CreateModifyOrderViewModel()
    {
        var requestPayload = new CreateOrderPayload
        {
            Content = new OrderContent(),
        };

        _request = new AtlasRequest
        {
            Status = RequestStatus.Draft,
            Payload = requestPayload,
        };

        _orderContent = requestPayload.Content;
        _realm = RealmService.GetMainThreadRealm();

        _realm.Write(() =>
        {
            _realm.Add(_request);
        });
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
            _request.Status = RequestStatus.Pending;
        });

        await Close();
    }

    [RelayCommand]
    public async Task Delete()
    {
        _realm.Write(() =>
        {
            _realm.Remove(_request);
        });

        await Close();
    }

    private async Task Close()
    {
        await Shell.Current.GoToAsync("..");
    }
}

