using System.Globalization;
using ObjectsAsAPI.Models;

namespace ObjectsAsAPI.Utils;

public class EnumToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is RequestStatus requestStatus)
        {
            return requestStatus switch
            {
                RequestStatus.Draft => Colors.Gray,
                RequestStatus.Pending => Colors.YellowGreen,
                RequestStatus.Handled => Colors.Green,
                _ => throw new NotImplementedException(),
            };
        }

        if (value is OrderStatus orderStatus)
        {
            return orderStatus switch
            {
                OrderStatus.Approved => Colors.Green,
                OrderStatus.Fulfilled => Colors.LightGreen,
                OrderStatus.Cancelled => Colors.Salmon,
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


