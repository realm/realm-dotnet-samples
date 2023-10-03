using ObjectsAsAPI.Views;

namespace ObjectsAsAPI;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("createOrder", typeof(CreateOrderPage));
        Routing.RegisterRoute("cancelOrder", typeof(CancelOrderPage));
        Routing.RegisterRoute("order", typeof(OrderPage));
    }
}