using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MigrationTutorial.Models;
using MigrationTutorial.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Realms;

namespace MigrationTutorial.Services
{
    public static class SeedData
    {
        public static void Seed()
        {
            var realm = RealmService.GetRealm();

#if SCHEMA_VERSION_1
            Migrations.V1Utils.SeedData();
            PrintDbToFile(realm, "AfterSeedingV1.json");
#endif

#if SCHEMA_VERSION_2
            Migrations.V2Utils.SeedData();
            PrintDbToFile(realm, "AfterSeedingAndMigrationV2.json");
#endif

#if SCHEMA_VERSION_3
            Migrations.V3Utils.SeedData();
            PrintDbToFile(realm, "AfterSeedingAndMigrationV3.json");
#endif
        }

        private static void PrintDbToFile(Realm realm, string? fileName)
        {
#if DEBUG
            var objectsDb = new List<RealmObject>();

#if SCHEMA_VERSION_1 || SCHEMA_VERSION_2 || SCHEMA_VERSION_3
            objectsDb.AddRange(realm.All<Consumable>().ToList());
            objectsDb.AddRange(realm.All<Employee>().ToList());
#endif

#if SCHEMA_VERSION_2 || SCHEMA_VERSION_3
            objectsDb.AddRange(realm.All<Customer>().ToList());
            objectsDb.AddRange(realm.All<Department>().ToList());
            objectsDb.AddRange(realm.All<Supplier>().ToList());
#endif

#if SCHEMA_VERSION_3
            objectsDb.AddRange(realm.All<MachineryAndTool>().ToList());
#endif

            // settings will automatically be used by JsonConvert.SerializeObject/DeserializeObject
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            var finalContent = new StringBuilder();
            foreach (var obj in objectsDb)
            {
                finalContent.Append(JsonConvert.SerializeObject(obj, new JsonConverter[] { new StringEnumConverter() } ));
            }

            var finalFileName = fileName ?? "printOutRealmContent.json";
            var absFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, finalFileName);
            File.WriteAllText(absFilePath, finalContent.ToString());
#endif
        }
    }

}