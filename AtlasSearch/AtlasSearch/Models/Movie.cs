using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AtlasSearch.Models;

[BsonIgnoreExtraElements]
public class Movie : IHighlightModel
{
    [BsonElement("_id")]
    public ObjectId Id { get; set; }

    [BsonElement("title")]
    public string Title { get; set; } = null!;

    [BsonElement("plot")]
    public string Plot { get; set; } = null!;

    [BsonElement("year")]
    public int Year { get; set; }

    [BsonElement("poster")]
    public string? PosterUrl { get; set; }

    [BsonElement("searchHighlights")]
    public Highlight[]? SearchHighlights { get; set; }
}
