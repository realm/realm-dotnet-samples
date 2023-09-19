# ObjectsAsAPI

**QuickJournalSync** shows how to integrate Realm and [Atlas Device Sync](https://www.mongodb.com/docs/atlas/app-services/sync/get-started/) in a sample MAUI application, with particular focus on error handling, connection state changes and offline realms. This app is a synced version of [`QuickJournal`](https://github.com/realm/realm-dotnet/tree/main/examples/QuickJournal), so check that out if you are mainly interested in how Realm can be used effectively in conjunction with MVVM and data binding.

The app allows the user to keep a very minimal synced journal, where each entry is made up of a title and a body. Every time a new journal entry is added or modified it gets persisted to a realm, and thanks to the bindings the UI gets updated immediately, with no additional code required. The app uses Device Sync to keep the journal entries synchronised with MongoDB Atlas and other devices. 

Additionally, the app is built to show some common use cases when dealing with synced application:
- [How to open a realm while offline](#offline-synced-realm)
- [How to react to connection changes](#connection-state-changes)
- [How to handle errors](#error-handling)

## Prerequisites

Developing .NET MAUI apps requires Visual Studio 2022 17.3 or greater, or Visual Studio 2022 for Mac 17.4 or greater. You also need to install the MAUI workload for the platform you are interested into. You can do this either through the Visual Studio UI or with the command line.

To install the workload with the command line:
- List all the available workloads with `dotnet workload search`
- Install a workload with `dotnet workload install *workloadName*`. 

For instance, if you're interested in the workload for all available platforms you can just do `dotnet workload install maui`. 

Once the workload is installed Visual Studio should ask you to install additional components if needed when the solution is opened. You can find additional information about how to install MAUI and support for specific platforms in the [official documentation](https://learn.microsoft.com/en-us/dotnet/maui/get-started/installation?tabs=vswin).

## Configuration

In order to run the example project you need to first create an App in [MongoDB Atlas](https://www.mongodb.com/docs/atlas/app-services/sync/get-started/) and change the `_appId` variable in `RealmService` to the App ID of the application you have created. Then, you can either configure the app manually, or import the App configuration in this repo.

### Import the App configuration

To import the app configuration in this repository you will need to:
- Set your App ID in ...
- Set your cluster name in ...
- Install `realm-cli` and generate an API key following the [documentation](https://www.mongodb.com/docs/atlas/app-services/cli/#mongodb-binary-bin.realm-cli).

    `npm install -g mongodb-realm-cli`
- Login to the realm-cli:

    `realm-cli login --api-key="<my api key>" --private-api-key="<my private api key>"`
- Go into the folder.....
- Deploy the app to Atlas App Services:

    `realm-cli push --yes`

sd

### Configure the App manually

To configure the app manually you will need to:
- Enable [Device Sync](https://www.mongodb.com/docs/atlas/app-services/sync/get-started/) on it.
- Enable [Development Mode](https://www.mongodb.com/docs/atlas/app-services/sync/configure/sync-settings/).
- Enable [Email/Password Authentication](https://www.mongodb.com/docs/atlas/app-services/authentication/email-password/).
- Create two new [Functions](https://www.mongodb.com/docs/atlas/app-services/functions/#define-a-function):
    - `CreateOrderRequestHandler`, using the code in .... 
    - `CancelOrderRequestHandler`, using the code in ....
- Create two new [Database Triggers](https://www.mongodb.com/docs/atlas/app-services/triggers/database-triggers/#create-a-database-trigger):
    - `CreateOrderRequestTrigger`, with operation type "Insert" and "Update" on the collection name `CreateOrderRequest` with "Full Document" checked, and selecting the `CreateOrderRequestHandler` function you have created in the previous step;
    - `CancelOrderRequestTrigger`, with operation type "Insert" and "Update" on the collection name `CancelOrderRequest` with "Full Document" checked, and selecting the `CancelOrderRequestHandler` function you have created in the previous step;
