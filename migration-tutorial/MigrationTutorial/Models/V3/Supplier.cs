#if SCHEMA_VERSION_3

using System.Collections.Generic;
using MongoDB.Bson;
using Realms;

namespace MigrationTutorial.Models
{
    public class Supplier : RealmObject
    {
        [PrimaryKey]
        public ObjectId Id { get; private set; } = ObjectId.GenerateNewId();

        public string Name { get; set; }

        private ISet<int> _SuppliedTypes { get; }

        [Ignored]
        public ISet<ConsumableType> SuppliedTypes { get; } = new HashSet<ConsumableType>();

        public void AddConsumableTypes(ConsumableType[] consumables)
        {
            foreach (var consumable in consumables)
            {
                AddConsumableType(consumable);
            }
        }

        public void AddConsumableType(ConsumableType consumable)
        {
            _SuppliedTypes.Add((int)consumable);
            SuppliedTypes.Add(consumable);
        }

        public void RemoveConsumableTypes(ConsumableType[] consumables)
        {
            foreach (var consumable in consumables)
            {
                RemoveConsumableType(consumable);
            }
        }

        public void RemoveConsumableType(ConsumableType consumable)
        {
            _SuppliedTypes.Remove((int)consumable);
            SuppliedTypes.Remove(consumable);
        }
    }
}

#endif