#if SCHEMA_VERSION_1

using System;
using System.Linq;
using MigrationTutorial.Models;
using MigrationTutorial.Services;
using MigrationTutorial.Utils;

namespace MigrationTutorial.Migrations
{
    public static class V1Utils
    {
        public static void SeedData()
        {
            var realm = RealmService.GetRealm();

            // since employees must be always there, if none, then the realm is just created
            if (realm.All<Employee>().Count() > 0)
            {
                Logger.LogWarning("The database was already seeded with V1");
                return;
            }

            Logger.LogInfo("Seed data: add Employees to realm");

            realm.Write(() =>
            {
                realm.Add(new Employee[]{
                    new Employee()
                    {
                        FullName = "Mario Rossi",
                        Age = 25,
                        Gender = "Male"
                    },
                    new Employee()
                    {
                        FullName = "Federica Bianchi",
                        Age = 23,
                        Gender = "Female"
                    },
                    new Employee()
                    {
                        FullName = "Luigi Verdi",
                        Age = 27,
                        Gender = "Male"
                    },
                    new Employee()
                    {
                        FullName = "Giovanni Viola",
                        Age = 29,
                        Gender = "Male"
                    }
                    });

                Logger.LogInfo("Seed data: Add Consumables to realm");

                realm.Add(new Consumable[]
                {
                    new Consumable()
                    {
                        Type = ConsumableType.Glue,
                        UnitOfMeasure = "Liters",
                        Price = 34,
                        ProductId = "sldjh39"
                    },
                    new Consumable()
                    {
                        Type = ConsumableType.Brush,
                        UnitOfMeasure = "Pieces",
                        Price = 10,
                        ProductId = "ms1qf"
                    },
                    new Consumable()
                    {
                        Type = ConsumableType.GlueHolder,
                        UnitOfMeasure = "Pieces",
                        Price = 39,
                        ProductId = "385uyyt"
                    },
                    new Consumable()
                    {
                        Type = ConsumableType.GlueHolder,
                        UnitOfMeasure = "Pieces",
                        Price = 9,
                        ProductId = "385uyyt"
                    },
                    new Consumable()
                    {
                        Type = ConsumableType.SandPaper,
                        UnitOfMeasure = "Strips",
                        Price = 120.5f,
                        ProductId = "210l4h"
                    },
                    new Consumable()
                    {
                        Type = ConsumableType.MaterialSheet,
                        UnitOfMeasure = "Pieces",
                        Price = 15,
                        ProductId = "1ddkptl5"
                    }
                });
            });
        }
    }
}

#endif