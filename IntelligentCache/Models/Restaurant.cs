using MongoDB.Bson;
using Realms;

namespace IntelligentCache.Models;

[MapTo("restaurants")]
public partial class Restaurant : IRealmObject
{
    [PrimaryKey]
    [MapTo("_id")]
    public ObjectId Id { get; private set; }

    [MapTo("name")]
    public string Name { get; private set; } = null!;

    [MapTo("borough")]
    public string Borough { get; private set; } = null!;

    [MapTo("cuisine")]
    private string _Cuisine { get; set; } = null!;

    public CuisineType Cuisine
    {
        get => Enum.Parse<CuisineType>(_Cuisine);
    }
}

public enum CuisineType
{
    American = 1,
    Italian = 2,
    Continental = 3,
    Bakery = 4,
    Seafood = 5,
}

