using System;
using System.Globalization;
using ObjectsAsAPI.Models;

namespace ObjectsAsAPI.Utils;

public class StatusConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is IRequest request)
        {
            return request.Status switch
            {
                RequestStatus.Draft => "Draft",
                RequestStatus.Pending => "Pending",
                RequestStatus.Approved => "Approved",
                RequestStatus.Rejected => $"Rejected{(string.IsNullOrEmpty(request.RejectedReason) ? "" : $": \"{request.RejectedReason}\"")}",
                _ => throw new NotImplementedException(),
            };
        }

        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

