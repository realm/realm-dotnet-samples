using AtlasSearch.Converters;
using AtlasSearch.Helpers;
using MongoDB.Bson.Serialization.Attributes;

namespace AtlasSearch.Models;

/// <summary>
/// An interface implemented by models that contain highlight information. It is used by <see cref="HighlightFormattedStringConverter"/>
/// to create a formatted string based on the matches.
/// </summary>
public interface IHighlightModel
{
    Highlight[]? SearchHighlights { get; }
}

public class Highlight
{
    [BsonElement("path")]
    public string Path { get; set; } = null!;

    [BsonElement("texts")]
    public HighlightText[] Texts { get; set; } = null!;

    [BsonElement("score")]
    public double Score { get; set; }
}

public class HighlightText
{
    [BsonElement("value")]
    public string Value { get; set; } = null!;

    [BsonElement("type")]
    [BsonSerializer(typeof(LowercaseEnumSerializer<HighlightTextType>))]
    public HighlightTextType Type { get; set; }
}

public enum HighlightTextType
{
    Hit,
    Text
}
