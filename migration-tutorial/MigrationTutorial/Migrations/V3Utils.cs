#if SCHEMA_VERSION_3

using System.Linq;
using MigrationTutorial.Services;
using MigrationTutorial.Models;
using Realms;
using Type = MigrationTutorial.Models.Type;
using MigrationTutorial.Utils;
using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace MigrationTutorial.Migrations
{
    public static class V3Utils
    {
        public static void SeedData()
        {
            var realm = RealmService.GetRealm();

            // where 2 is just the 2 items that have moved from Consumable to MachineryAndTool => brush and glue holder
            if (realm.All<MachineryAndTool>().Count() > 2)
            {
                Logger.LogWarning("The database was already seeded with V3");
                return;
            }

            var headWorkshop = realm.All<Employee>().Where(e => e.FullName == "Giovanni Viola").FirstOrDefault();
            var workshopDep = new Department()
            {
                Name = "Workshop",
                Head = headWorkshop
            };

            realm.Write(() =>
            {
                Logger.LogInfo("Seed data: add Workshop department");

                realm.Add(workshopDep);
                headWorkshop.Department = workshopDep;

                Logger.LogInfo("Seed data: add MachineryAndTools");

                realm.Add(new MachineryAndTool[]
                {
                    new MachineryAndTool()
                    {
                        Type = Type.ManufacturingMachine,
                        Status = OperationalStatus.Functioning,
                        AssignedMaintainer = null,
                        ToolName = "Milling machine"
                    },
                    new MachineryAndTool()
                    {
                        Type = Type.ManufacturingMachine,
                        Status = OperationalStatus.Functioning,
                        AssignedMaintainer = null,
                        ToolName = "Press"
                    },
                    new MachineryAndTool()
                    {
                        Type = Type.PrototypingMachine,
                        Status = OperationalStatus.Functioning,
                        AssignedMaintainer = null,
                        ToolName = "Grinder"
                    }
                });
            });
        }

        public static void DoMigrate(Migration migration, ulong oldSchemaVersion)
        {
            if (oldSchemaVersion < 2)
            {
                Logger.LogInfo("In migration: converting Employee's gender from string to enum");

                var newEmployees = migration.NewRealm.All<Employee>();
                var oldEmployees = migration.OldRealm.DynamicApi.All("Employee");

                for (var i = 0; i < newEmployees.Count(); i++)
                {
                    var newEmployee = newEmployees.ElementAt(i);
                    var oldEmployee = oldEmployees.ElementAt(i);
                    if (string.Equals(oldEmployee.Gender, "female", StringComparison.OrdinalIgnoreCase))
                    {
                        newEmployee.Gender = Gender.Female;
                    }
                    else if (string.Equals(oldEmployee.Gender, "male", StringComparison.OrdinalIgnoreCase))
                    {
                        newEmployee.Gender = Gender.Male;
                    }
                    else
                    {
                        newEmployee.Gender = Gender.Other;
                    }
                }


                Logger.LogInfo("In migration: rename Consumable.Price to Consumable.LastPurchasedPrice");

                migration.RenameProperty(nameof(Consumable), "Price", nameof(Consumable.LastPurchasedPrice));

                Logger.LogInfo("In migration: remove duplicated Consumable to accomodate ProductId to become the new primary key");

                var newConsumables = migration.NewRealm.All<Consumable>();
                var distinctConsumableId = new HashSet<string>();
                var consumableToDelete = new List<Consumable>();

                for (var i = 0; i < newConsumables.Count(); i++)
                {
                    var currConsumable = newConsumables.ElementAt(i);

                    // remove duplicates since ProductId is the new PrimaryKey
                    if (distinctConsumableId.Contains(currConsumable.ProductId))
                    {
                        consumableToDelete.Add(currConsumable);
                    }
                    else
                    {
                        distinctConsumableId.Add(currConsumable.ProductId);
                    }
                }

                consumableToDelete.ForEach(x => migration.NewRealm.Remove(x));
            }

            Logger.LogInfo("In migration: convert GlueHolder from a Consumable to a MachineryAndTool");
            ConvertConsumableToTool(migration, oldSchemaVersion, "GlueHolder");

            Logger.LogInfo("In migration: convert Brush from a Consumable to a MachineryAndTool");
            ConvertConsumableToTool(migration, oldSchemaVersion, "Brush");
        }

        private static void ConvertConsumableToTool(Migration migration, ulong oldSchemaVersion, string consumableType)
        {
            // it's assumed that there's always 1 and 1 only type of Consumable
            var oldConsumable = ((IQueryable<RealmObject>)migration.OldRealm.DynamicApi.All("Consumable")).Filter("_Type == $0", consumableType).FirstOrDefault();
            if (oldConsumable == null)
            {
                Logger.LogWarning($"No consumable was found with type {consumableType}. Nothing to convert.");
                return;
            }

            Supplier consumableSupplier = null;
            string consumableBrand = string.Empty;

            // no suppliers if coming from schemaVersion 1
            if (oldSchemaVersion > 1)
            {
                var supplierId = oldConsumable.DynamicApi.Get<RealmObject>("Supplier").DynamicApi.Get<ObjectId>("Id");
                consumableSupplier = migration.NewRealm.All<Supplier>().Filter("Id == $0", supplierId).FirstOrDefault();
                consumableBrand = oldConsumable.DynamicApi.Get<string>("Brand");
            }

            migration.NewRealm.Add(new MachineryAndTool()
            {
                Type = Type.ManufacturingTool,
                Status = OperationalStatus.Functioning,
                AssignedMaintainer = null,
                ToolName = oldConsumable.DynamicApi.Get<string>("_Type"),
                Supplier = consumableSupplier,
                Brand = consumableBrand
            });

            var newConsumables = migration.NewRealm.All<Consumable>();

            // ProductId could be empty because of human error
            var consumableProductId = oldConsumable.DynamicApi.Get<string>("ProductId");
            if (consumableProductId != string.Empty)
            {
                var consumableToRemove = newConsumables.Where(x => x.ProductId == consumableProductId).First();
                migration.NewRealm.Remove(consumableToRemove);
            }
        }
    }
}

#endif