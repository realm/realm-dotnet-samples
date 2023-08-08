using System.Reflection;
using AtlasSearch.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AtlasSearch.Helpers;

public static class ProjectionHelper
{
    /// <summary>
    /// This is a helper method that construct a projection document from the properties of an object. It will include all public properties of that object in the projection.
    /// </summary>
    public static BsonDocument GetProjection<T>(bool includeId = true)
    {
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => p.GetCustomAttribute<BsonElementAttribute>()?.ElementName ?? p.Name);

        var result = new BsonDocument();
        foreach (var property in properties)
        {
            result[property] = true;
        }

        result["_id"] = includeId;

        if (typeof(IHighlightModel).IsAssignableFrom(typeof(T)))
        {
            result["searchHighlights"] = new BsonDocument("$meta", "searchHighlights");
        }

        return result;
    }
}