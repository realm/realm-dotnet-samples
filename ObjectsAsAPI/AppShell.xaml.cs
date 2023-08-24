using ObjectsAsAPI.Views;

namespace ObjectsAsAPI;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

        Routing.RegisterRoute("createModifyOrder", typeof(CreateModifyOrderPage));
    }
}

