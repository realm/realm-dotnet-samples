# AtlasSearch

This example MAUI application shows how to use Realm to run Atlas Search queries by using [the Mongo client](https://www.mongodb.com/docs/realm/sdk/dotnet/app-services/mongodb-remote-access/) built into the Realm SDK. It uses the aggregation pipeline to build a search query on a sample dataset.

It includes two demos based on the [Autocomplete](https://www.mongodb.com/docs/atlas/atlas-search/tutorial/autocomplete-tutorial/) and [GeoJson](https://www.mongodb.com/docs/atlas/atlas-search/tutorial/run-geo-query/) tutorials for Atlas Search.

## Prerequisites

Developing .NET MAUI apps requires Visual Studio 2022 17.3 or greater, or Visual Studio 2022 for Mac 17.4 or greater. You also need to install the MAUI workload for the platform you are interested into. You can do this either through the Visual Studio UI or with the command line.

To install the workload with the command line:
- List all the available workloads with `dotnet workload search`
- Install a workload with `dotnet workload install *workloadName*`.

For instance, if you're interested in the workload for all available platforms you can just do `dotnet workload install maui`.

Once the workload is installed Visual Studio should ask you to install additional components if needed when the solution is opened. You can find additional information about how to install MAUI and support for specific platforms in the [official documentation](https://learn.microsoft.com/en-us/dotnet/maui/get-started/installation?tabs=vswin).

## Configuration

In order to run the example project you need to:
- Create an Atlas Cluster and follow the [load sample data](https://www.mongodb.com/docs/atlas/sample-data/) instructions to load the `sample_airbnb` and `sample_mflix` datasets.
- Complete the [Define an Index with Autocomplete](https://www.mongodb.com/docs/atlas/atlas-search/tutorial/autocomplete-tutorial/#define-an-index-with-autocomplete) and [Create the Atlas \[GeoJson\] Search Index](https://www.mongodb.com/docs/atlas/atlas-search/tutorial/run-geo-query/#create-the-fts-index) sections from the Atlas Search tutorials.
- Create an Atlas App Services application and set its app id in config.json
- Enable [Anonymous Authentication](https://www.mongodb.com/docs/atlas/app-services/authentication/anonymous/). The example project use anonymous authentication for simplicity, but feel free to use the authentication provider you prefer.

## Autocomplete

The autocomplete demo simulates autocompleting a movie based on its title. The user can then click on a movie to view more details about it. The aggregation pipeline used looks like:

```js
[{
    $search: {
        autocomplete: {
            path: "title",
            query: "<user-input-query>"
        },
        highlight: {
            path: "title"
        },
    }
}, {
    $limit: 10,
}, {
    $project: {
        title: true,
        // ...
    }
}]
```

The `highlight` option allows you to visualize the matched characters in the UI - see the [docs](https://www.mongodb.com/docs/atlas/atlas-search/highlighting/) for more details.

## Compound

The compound demo simulates searching for a property based on keywords and location. It uses the [compound operator](https://www.mongodb.com/docs/atlas/atlas-search/compound/) to combine a [geowithin](https://www.mongodb.com/docs/atlas/atlas-search/geoWithin/) and [phrase](https://www.mongodb.com/docs/atlas/atlas-search/phrase/) queries. The final pipeline looks like:

```js
[{
    $search: {
        compound: {
            must: [{
                geoWithin: {
                    path: "address.location",
                    circle: {
                        center: {
                            type: "Point",
                            coordinates: [ /* user-supplied longitude, user-supplid latitude */ ]
                        },
                        radius: /* user-supplied distance */ 123
                    }
                }
            }],
            should: [{
                phrase: {
                    path: "description",
                    query: "<user-input-query>"
                }
            }]
        },
        highlight: {
            path: "description"
        }
    }
}, {
    $limit: 10
}, {
    $project: {
        name: true,
        description: true,
        "address.location": true,
        "address.street": true,
        searchHighlights: { $meta: "searchHighlights" }
    }
}]
```

The results are visualized on a map based on the address and listed in a listview with their details.

## Helpers

### HighlightFormattedStringConverter

This is a value converter that converts a collection of highlights to a formatted string so that matched terms are displayed in bold. It does that by iterating the collection of highlights and creating spans and formatting them based on whether the highlight type is `hit` or `text`.

### ProjectionHelper

This is a helper class that creates a projection based on a strongly-typed model (such as `Movie.cs` or `Listing.cs`). By default, it includes all public properties and creates a document like:

```js
{
    prop1: true,
    prop2: true,
    // ...
}
```

### DebounceHelper

This is a simple helper that [debounces](https://levelup.gitconnected.com/debounce-in-javascript-improve-your-applications-performance-5b01855e086) changes to the search query so that we make excessive http requests while a user is typing.