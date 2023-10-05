using System.Globalization;
using ObjectsAsAPI.Models;

namespace ObjectsAsAPI.Utils;

public class RequestToDescriptionConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values[0] is IRequest req)
        {
            string requestType = null!;
            string? orderIdentifier = null;

            if (req is CreateOrderRequest createReq)
            {
                requestType = "CreateOrder";
                orderIdentifier = createReq.Content?.OrderName;
            }
            else if (req is CancelOrderRequest cancReq)
            {
                requestType = "CancelOrder";
                orderIdentifier = cancReq.OrderId.ToString();
            }

            if (orderIdentifier?.Length > 10)
            {
                orderIdentifier = orderIdentifier[..10];
            }

            var status = req.Status switch
            {
                RequestStatus.Approved => "✅ ",
                RequestStatus.Rejected => "❌ ",
                _ => string.Empty,
            };

            return $"{status}{requestType}{(string.IsNullOrEmpty(orderIdentifier) ? "" : $" - {orderIdentifier}")}";
        }

        return true;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

