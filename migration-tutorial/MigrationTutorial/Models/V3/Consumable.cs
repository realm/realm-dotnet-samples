#if SCHEMA_VERSION_3

using System;
using Realms;

namespace MigrationTutorial.Models
{
    public class Consumable : RealmObject
    {
        [PrimaryKey]
        public string ProductId { get; private set; }

        public ConsumableType Type
        {
            get => Enum.Parse<ConsumableType>(_Type);
            set => _Type = value.ToString();
        }

        [Required]
        private string _Type { get; set; } = string.Empty;

        public int Quantity { get; set; } = 0;

        [Required]
        public string UnitOfMeasure { get; set; }

        public Supplier Supplier { get; set; }

        public float LastPurchasedPrice { get; set; }

        public string Brand { get; set; }

        private Consumable() { }

        public Consumable(string productId = "")
        {
            ProductId = productId;
        }
    }


    public enum ConsumableType
    {
        Glue,
        SandPaper,
        MaterialSheet
    }
}

#endif