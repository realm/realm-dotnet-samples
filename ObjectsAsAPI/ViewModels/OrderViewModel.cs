using System;
using CommunityToolkit.Mvvm.ComponentModel;
using ObjectsAsAPI.Models;

namespace ObjectsAsAPI.ViewModels;

[QueryProperty("Order", nameof(Order))]
public partial class OrderViewModel : BaseViewModel
{
    [ObservableProperty]
    private Order _order = null!;
}

