
#if SCHEMA_VERSION_3

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

        public Department Department { get; set; }

        public Gender Gender
        {
            get => Enum.Parse<Gender>(_Gender);
            set => _Gender = value.ToString();
        }

        private string _Gender { get; set; }
    }

    public enum Gender
    {
        Male,
        Female,
        Other
    }
}

#endif