﻿using CommunityToolkit.Mvvm.ComponentModel;
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

        Orders = _realm.All<Order>();
        Requests = _realm.All<AtlasRequest>();

        if (!Orders.Any())
        {
            var orders = new List<Order>
            {
                new Order
                {
                    Status = OrderStatus.Processing,
                    Content = new OrderContent
                    {
                         OrderName = "Order1",
                         CreatedAt = DateTimeOffset.Now,
                    }
                },
                new Order
                {
                    Status = OrderStatus.Approved,
                    Content = new OrderContent
                    {
                         OrderName = "Order2",
                         CreatedAt = DateTimeOffset.Now,
                    }
                }
            };
            _realm.Write(() =>
            {
                _realm.Add(orders);
            });
        }
    }

    [RelayCommand]
    public async Task AddOrder()
    {
        await Shell.Current.GoToAsync($"createModifyOrder");
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
}
