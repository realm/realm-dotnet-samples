using MongoDB.Bson;
using Realms;

namespace IntelligentCache.Models;

[MapTo("restaurantsRequests")]
public partial class RestaurantsRequest : IAsymmetricObject
{
    [PrimaryKey]
    [MapTo("_id")]
    public ObjectId Id { get; private set; }

    [MapTo("cuisine")]
    public string Cuisine { get; private set; }

    [MapTo("timestamp")]
    public DateTimeOffset Timestamp { get; private set; }

    public RestaurantsRequest(CuisineType cuisine)
    {
        Id = ObjectId.GenerateNewId();
        Cuisine = cuisine.ToString();
        Timestamp = DateTimeOffset.Now;
    }
}

