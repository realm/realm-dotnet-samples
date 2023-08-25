using System.Globalization;
using AtlasSearch.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace AtlasSearch.Converters;

/// <summary>
/// A value converter that creates a formatted string based on whether a portion of the string is a match or not. This is using the higlights collection
/// returned in a <see cref="IHighlightModel"/> as well as the original query.
/// </summary>
public class HighlightFormattedStringConverter : IMultiValueConverter
{
    public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        var model = values[0] as IHighlightModel;
        var searchQuery = values.ElementAtOrDefault(1) as string;
        var path = parameter as string ?? throw new Exception("path must be supplied as converter parameter");

        if (model == null)
        {
            return null;
        }

        var property = values[0]?.GetType().GetProperty(path);
        if (property == null)
        {
            throw new Exception("path must point to a valid property");
        }

        var result = new FormattedString();
        if (model.SearchHighlights != null)
        {
            var title = property.GetCustomAttribute<BsonElementAttribute>()?.ElementName ?? path;
            var highlight = model.SearchHighlights.FirstOrDefault(m => m.Path == title);
            if (highlight != null)
            {
                foreach (var text in highlight.Texts)
                {
                    switch (text.Type)
                    {
                        case HighlightTextType.Hit:
                            if (searchQuery != null)
                            {
                                result.Spans.Add(CreateSpan(text.Value.Substring(0, searchQuery.Length), isHit: true));
                                result.Spans.Add(CreateSpan(text.Value.Substring(searchQuery.Length), isHit: false));
                            }
                            else
                            {
                                result.Spans.Add(CreateSpan(text.Value, isHit: true));
                            }

                            break;
                        case HighlightTextType.Text:
                            result.Spans.Add(CreateSpan(text.Value, isHit: false));
                            break;
                    }
                }
            }
        }
        else
        {
            result.Spans.Add(CreateSpan(property.GetValue(values[0]) as string, false));
        }

        return result;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    private static Span CreateSpan(string? text, bool isHit)
    {
        return new Span
        {
            Text = text,
            TextColor = isHit ? HitColor : AutocompleteColor,
            FontAttributes = isHit ? FontAttributes.Bold : FontAttributes.None,
        };
    }

    private static Color HitColor => Application.Current?.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black;

    private static Color AutocompleteColor => Application.Current?.RequestedTheme == AppTheme.Dark ? Colors.LightGray : Colors.DarkGray;
}