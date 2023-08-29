using ObjectsAsAPI.Models;
using Realms;
using Realms.Sync;
using System.Diagnostics.CodeAnalysis;

namespace ObjectsAsAPI.Services;


public static class RealmService
{
    private static readonly string _appId = "";

    private static bool _serviceInitialised;

    private static Realms.Sync.App? _app;

    private static Realm? _mainThreadRealm;

    public static User? CurrentUser => _app?.CurrentUser;

    public static void Init()
    {
        if (_serviceInitialised)
        {
            return;
        }

        if (string.IsNullOrEmpty(_appId))
        {
            throw new Exception("Remember to add your appId!");
        }

        var appConfiguration = new AppConfiguration(_appId);

        _app = Realms.Sync.App.Create(appConfiguration);

        _serviceInitialised = true;
    }

    public static Realm GetMainThreadRealm()
    {
        if (!MainThread.IsMainThread)
        {
            throw new InvalidOperationException("This method should be called only from the main thread!");
        }

        _mainThreadRealm ??= Realm.GetInstance(GetRealmConfig());

        return _mainThreadRealm;
    }

    public static Realm GetBackgroundThreadRealm() => Realm.GetInstance(GetRealmConfig());

    public static async Task RegisterAsync(string email, string password)
    {
        CheckIfInitialized();

        await _app.EmailPasswordAuth.RegisterUserAsync(email, password);
    }

    public static async Task LoginAsync(string email, string password)
    {
        CheckIfInitialized();

        await _app.LogInAsync(Credentials.EmailPassword(email, password));

        // After logging in we want to wait for synchronization to happen
        using var realm = await Realm.GetInstanceAsync(GetRealmConfig());
    }

    public static async Task LogoutAsync()
    {
        CheckIfInitialized();

        if (CurrentUser == null)
        {
            return;
        }

        await CurrentUser.LogOutAsync();

        if (_mainThreadRealm is not null)
        {
            _mainThreadRealm.Dispose();
            _mainThreadRealm = null;
        }
    }

    [MemberNotNull(nameof(_app))]
    private static void CheckIfInitialized()
    {
        if (_app == null)
        {
            throw new InvalidOperationException("Remember to initialize RealmService!");
        }
    }

    private static FlexibleSyncConfiguration GetRealmConfig()
    {
        if (CurrentUser == null)
        {
            throw new InvalidOperationException("Cannot get Realm config before login!");
        }

        var config = new FlexibleSyncConfiguration(CurrentUser)
        {
            PopulateInitialSubscriptions = (realm) =>
            {
                var myOrders = realm.All<Order>().Where(r => r.CreatorId == CurrentUser.Id);
                var myRequests = realm.All<AtlasRequest>().Where(r => r.CreatorId == CurrentUser.Id);
                var myCreateOrderPayload = realm.All<CreateOrderPayload>().Where(r => r.CreatorId == CurrentUser.Id);
                var myCreateOrderResponse = realm.All<CreateOrderResponse>().Where(r => r.CreatorId == CurrentUser.Id);
                //TODO Here we need the other kinds too

                realm.Subscriptions.Add(myOrders);
                realm.Subscriptions.Add(myRequests);
                realm.Subscriptions.Add(myCreateOrderPayload);
                realm.Subscriptions.Add(myCreateOrderResponse);
            },
        };

        return config;
    }
}