using System;
using System.IO;
using Realms;
using MigrationTutorial.Migrations;
using MigrationTutorial.Utils;

namespace MigrationTutorial.Services
{
    public class RealmService
    {
        private static ulong _schemaVersion = 0;

        private static RealmConfiguration _realmConfiguration;

        public static Realm GetRealm() => Realm.GetInstance(_realmConfiguration);

        public static void Init()
        {
            if (_schemaVersion == 0)
            {
#if SCHEMA_VERSION_1
                _schemaVersion = 1;
#elif SCHEMA_VERSION_2
                _schemaVersion = 2;
#elif SCHEMA_VERSION_3
                _schemaVersion = 3;
#endif

                Logger.LogInfo($"Selected schema version: {_schemaVersion}");

                _realmConfiguration = new RealmConfiguration("migrationTutorial.realm")
                {
                    SchemaVersion = _schemaVersion,

                    MigrationCallback = (migration, oldSchemaVersion) =>
                    {
                        Logger.LogInfo("A migration has started");

#if SCHEMA_VERSION_2
                        Logger.LogInfo("The migration V2 is about to take place");
                        V2Utils.DoMigrate(migration, oldSchemaVersion);
#elif SCHEMA_VERSION_3
                        Logger.LogInfo("The migration V3 is about to take place");
                        V3Utils.DoMigrate(migration, oldSchemaVersion);
#endif
                    }
                };

                var realmPath = _realmConfiguration.DatabasePath;
                if (File.Exists(realmPath) && _schemaVersion == 1)
                {
                    try
                    {
                        Logger.LogDebug($"Since the realm already exists and the supplied schema version is 1, it's assumed that you want to start from scratch.\n       Deleting {realmPath}");
                        Realm.DeleteRealm(_realmConfiguration);
                        Logger.LogInfo($"Realm is going to be created at:\n       {realmPath}");
                    }
                    catch (Exception e)
                    {
                        Logger.LogWarning($"It was not possible to delete the local realm at path {realmPath} because of an exception\n{e}");
                    }
                }
                else
                {
                    Logger.LogInfo($"the Realm is located at:\n       {realmPath}");
                }
            }
            else
            {
                Logger.LogWarning($"You can't set the schema version more than once! It's currently set to {_schemaVersion}.");
            }
        }
    }
}
