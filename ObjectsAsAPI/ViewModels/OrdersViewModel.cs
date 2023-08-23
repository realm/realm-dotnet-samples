using CommunityToolkit.Mvvm.ComponentModel;
using ObjectsAsAPI.Models;

namespace ObjectsAsAPI.ViewModels;

public partial class OrdersViewModel : BaseViewModel
{
    [ObservableProperty]
    private IList<Order> _orders; 

    public OrdersViewModel()
    {
        _orders = new List<Order>
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
    }
}

