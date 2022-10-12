#if SCHEMA_VERSION_1

using System;
using MongoDB.Bson;
using Realms;

namespace MigrationTutorial.Models
{
    public class Employee : RealmObject
    {
        [PrimaryKey]
        public ObjectId Id { get; private set; } = ObjectId.GenerateNewId();

        [Required]
        public string FullName { get; set; }

        [Required]
        public int? Age { get; set; }

        public string Gender { get; set; }
    }
}

#endif