using Realms;
using Realms.Sync;
using Realms.Sync.ErrorHandling;
using System.Diagnostics.CodeAnalysis;
using Credentials = Realms.Sync.Credentials;

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
        return Realm.GetInstance(); //TODO For testing

        if (!MainThread.IsMainThread)
        {
            throw new InvalidOperationException("This method should be called only from the main thread!");
        }

        if (_mainThreadRealm is null)
        {
            _mainThreadRealm = Realm.GetInstance(GetRealmConfig());
        }

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

        // Creates a CancellationTokenSource that will be cancelled after 4 seconds.
        var cts = new CancellationTokenSource(4000);

        try
        {
            using var realm = await Realm.GetInstanceAsync(GetRealmConfig(), cts.Token);
        }
        catch (TaskCanceledException)
        {
            // If there are connectivity issues, or the synchronization is taking too long we arrive here
        }
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

    private static FlexibleSyncConfiguration GetRealmConfig(SyncConfigurationBase.SessionErrorCallback? sessionErrorCallback = null,
        ClientResetHandlerBase? clientResetHandler = null)
    {
        if (CurrentUser == null)
        {
            throw new InvalidOperationException("Cannot get Realm config before login!");
        }

        var config = new FlexibleSyncConfiguration(CurrentUser)
        {
            PopulateInitialSubscriptions = (realm) =>
            {
                //var query = realm.All<JournalEntry>().Where(j => j.UserId == CurrentUser.Id);
                //realm.Subscriptions.Add(query, new SubscriptionOptions { Name = "myEntries" });
            },
        };

        return config;
    }

    private static void LogAndShowToast(string text)
    {
        Console.WriteLine(text);
        MainThread.BeginInvokeOnMainThread(async () => await DialogService.ShowToast(text));
    }
}