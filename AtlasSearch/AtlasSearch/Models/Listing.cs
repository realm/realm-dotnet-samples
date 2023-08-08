using MongoDB.Bson.Serialization.Attributes;

using MauiLocation = Microsoft.Maui.Devices.Sensors.Location;

namespace AtlasSearch.Models;

[BsonIgnoreExtraElements]
public partial class Listing : IHighlightModel
{
    [BsonElement("name")]
    public string Name { get; set; } = null!;

    [BsonElement("description")]
    public string Description { get; set; } = null!;

    [BsonElement("address")]
    public Address Address { get; set; } = null!;

    [BsonElement("searchHighlights")]
    public Highlight[]? SearchHighlights { get; set; }
}

[BsonIgnoreExtraElements]
public class Address
{
    [BsonElement("street")]
    public string Street { get; set; } = null!;

    [BsonElement("location")]
    public Location Location { get; set; } = null!;

    public string StringAddress => $"📌 {Street}";

    public MauiLocation MauiLocation => new(Location.Latitude, Location.Longitude);
}

[BsonIgnoreExtraElements]
public class Location
{
    [BsonElement("coordinates")]
    private double[] Coordinates { get; set; } = null!;

    public double Longitude => Coordinates[0];

    public double Latitude => Coordinates[1];
}