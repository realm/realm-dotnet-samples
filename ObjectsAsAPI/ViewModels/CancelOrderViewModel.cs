using System;
using CommunityToolkit.Mvvm.ComponentModel;
using ObjectsAsAPI.Models;

namespace ObjectsAsAPI.ViewModels;

[QueryProperty("Request", "Request")]
public partial class CancelOrderViewModel: BaseViewModel
{
    [ObservableProperty]
    private CancelOrderRequest _request = null!;
}

