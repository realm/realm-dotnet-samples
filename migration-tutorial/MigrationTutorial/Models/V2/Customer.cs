#if SCHEMA_VERSION_2

using System;
using MongoDB.Bson;
using Realms;

namespace MigrationTutorial.Models
{
    public class Customer : RealmObject
    {
        [PrimaryKey]
        public ObjectId Id { get; private set; } = ObjectId.GenerateNewId();

        [Required]
        public string Name { get; set; }

        public string Location { get; set; }
    }
}

#endif