using System.Text.Json;
using AtlasSearch.Helpers;
using AtlasSearch.Models;
using MongoDB.Bson;
using Realms.Sync;

using Location = Microsoft.Maui.Devices.Sensors.Location;
using RealmApp = Realms.Sync.App;

namespace AtlasSearch.Services;

public static class SearchService
{
    private static readonly Lazy<Task> _initializeTask = new(InitializeCore);

    private static RealmApp _app = null!;
    private static MongoClient.Collection<Movie> _movieCollection = null!;
    private static MongoClient.Collection<Listing> _listingCollection = null!;

    public static bool IsInitialized => _initializeTask is { IsValueCreated: true, Value.IsCompleted: true };

    public static Task Initialize() => _initializeTask.Value;

    private static async Task InitializeCore()
    {
        using var configStream = await FileSystem.Current.OpenAppPackageFileAsync("config.json");

        var config = JsonSerializer.Deserialize<Config>(configStream)!;

        if (string.IsNullOrEmpty(config.AppId) || config.AppId == "<fill-me-in>")
        {
            throw new Exception("Invalid app Id: make sure to fill in your app id in config.json. For more information, check the Configuration section of README.md.");
        }

        _app = RealmApp.Create(new AppConfiguration(config.AppId)
        {
            BaseUri = new Uri(config.ServerUrl)
        });

        var user = _app.CurrentUser ?? await _app.LogInAsync(Credentials.Anonymous());
        try
        {
            await user.RefreshCustomDataAsync();
        }
        catch
        {
            await user.LogOutAsync();
            user = await _app.LogInAsync(Credentials.Anonymous());
        }

        var client = user.GetMongoClient(config.ServiceName);

        _movieCollection = client.GetDatabase("sample_mflix")
            .GetCollection<Movie>("movies");

        _listingCollection = client.GetDatabase("sample_airbnb")
            .GetCollection<Listing>("listingsAndReviews");
    }

    public static Task<Movie[]> Autocomplete(string query)
    {
        var searchStage = new BsonDocument
        {
            ["autocomplete"] = new BsonDocument
            {
                ["path"] = "title",
                ["query"] = query,
            },
            ["highlight"] = new BsonDocument
            {
                ["path"] = "title",
            }
        };

        return _movieCollection.AggregateAsync<Movie>(
            new BsonDocument("$search", searchStage),
            new BsonDocument("$limit", 10),
            new BsonDocument("$project", ProjectionHelper.GetProjection<Movie>(includeId: false)));
    }

    public static Task<Listing[]> Compound(string query, Location center, double distance)
    {
        // GetProjection is a helper method that creates a bson document with all public fields
        // of the model.
        var projection = ProjectionHelper.GetProjection<Listing>(includeId: false);

        // Alternatively, the projection can be defined manually
        //// projection = new BsonDocument
        //// {
        ////     ["name"] = true,
        ////     ["description"] = true,
        ////     ["address"] = true,
        ////     ["searchHighlights"] = new BsonDocument("$meta", "searchHighlights"),
        //// };

        var searchStage = new BsonDocument
        {
            ["compound"] = new BsonDocument
            {
                ["must"] = new BsonArray
                {
                    new BsonDocument
                    {
                        ["geoWithin"] = new BsonDocument
                        {
                            ["path"] = "address.location",
                            ["circle"] = new BsonDocument
                            {
                                ["center"] = new BsonDocument
                                {
                                    ["type"] = "Point",
                                    ["coordinates"] = new BsonArray { center.Longitude, center.Latitude }
                                },
                                ["radius"] = distance
                            }
                        }
                    }
                },
                ["should"] = new BsonArray
                {
                    new BsonDocument
                    {
                        ["phrase"] = new BsonDocument
                        {
                            ["path"] = "description",
                            ["query"] = query
                        }
                    }
                }
            },
            ["highlight"] = new BsonDocument
            {
                ["path"] = "description"
            }
        };

        return _listingCollection.AggregateAsync<Listing>(
            new BsonDocument("$search", searchStage),
            new BsonDocument("$limit", 10),
            new BsonDocument("$project", projection));
    }

    private class Config
    {
        public string? AppId { get; set; }

        public string ServerUrl { get; set; } = "https://realm.mongodb.com";

        public string ServiceName { get; set; } = "mongodb-atlas";
    }
}
