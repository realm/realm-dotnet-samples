#if SCHEMA_VERSION_2

using System;
using System.Collections.Generic;
using System.Linq;
using MigrationTutorial.Models;
using MigrationTutorial.Services;
using MigrationTutorial.Utils;
using Realms;

namespace MigrationTutorial.Migrations
{
    public static class V2Utils
    {
        public static void SeedData()
        {
            var realm = RealmService.GetRealm();

            if (realm.All<Customer>().Count() > 0)
            {
                Logger.LogWarning("The database was already seeded with V2");
                return;
            }

            var employees = realm.All<Employee>();
            var headManufacturing = employees.Where(e => e.FullName == "Mario Rossi").FirstOrDefault();
            var headPrototyping = employees.Where(e => e.FullName == "Federica Bianchi").FirstOrDefault();
            var manufacturinDep = new Department()
            {
                Name = "Manufacturing",
                Head = headManufacturing
            };
            var prototypingDep = new Department()
            {
                Name = "Prototyping",
                Head = headPrototyping
            };
            Logger.LogInfo("Seed data: add Suppliers to realm");

            realm.Write(() =>
            {
                var suppliers = new List<Supplier>();

                var supplier = new Supplier() { Name = "RoseWood" };
                supplier.AddConsumableTypes(new ConsumableType[]
                {
                    ConsumableType.Brush,
                    ConsumableType.Glue,
                    ConsumableType.GlueHolder
                });
                suppliers.Add(supplier);

                supplier = new Supplier() { Name = "Catalina" };
                supplier.AddConsumableTypes(new ConsumableType[]
                {
                    ConsumableType.SandPaper,
                    ConsumableType.Brush
                });
                suppliers.Add(supplier);

                supplier = new Supplier() { Name = "Montgomery" };
                supplier.AddConsumableType(ConsumableType.MaterialSheet);
                suppliers.Add(supplier);

                realm.Add(suppliers);

                Logger.LogInfo("Seed data: add Departments to realm");

                realm.Add(new Department[] { manufacturinDep, prototypingDep });
                headPrototyping.Department = prototypingDep;
                headManufacturing.Department = manufacturinDep;

                Logger.LogInfo("Seed data: add Customers to realm");

                realm.Add(new Customer[]{
                    new Customer()
                    {
                        Name = "Nuke",
                        Location = "United States"
                    },
                    new Customer()
                    {
                        Name = "Gacci",
                        Location = "Italy"
                    }
                });
            });

            var manufactDep = realm.All<Department>().Where(d => d.Name == "Manufacturing").First();
            var suppliers = realm.All<Supplier>();
            var consumables = realm.All<Consumable>();

            realm.Write(() =>
            {
                Logger.LogInfo("Seed data: assign all Employees' Department to Manufacturing");

                for (var i = 0; i < employees.Count(); i++)
                {
                    employees.ElementAt(i).Department = manufactDep;
                }

                Logger.LogInfo("Seed data: match Consumables with the right Suppliers and store it");

                for (var i = 0; i < consumables.Count(); i++)
                {
                    // it is expected to have a supplier for everything
                    var typeToSearch = consumables.ElementAt(i).Type;
                    var matchingSupplier = suppliers.Filter("ANY _SuppliedTypes = $0", typeToSearch.ToString()).First();
                    consumables.ElementAt(i).Supplier = matchingSupplier;
                }
            });
        }

        public static void DoMigrate(Migration migration, ulong oldSchemaVersion)
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
    }
}

#endif