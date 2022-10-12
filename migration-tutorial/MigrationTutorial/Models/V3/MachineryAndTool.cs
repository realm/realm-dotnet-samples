#if SCHEMA_VERSION_3

using System;
using MongoDB.Bson;
using Realms;

namespace MigrationTutorial.Models
{
    public class MachineryAndTool : RealmObject
    {
        [PrimaryKey]
        public ObjectId Id { get; private set; } = ObjectId.GenerateNewId();

        private string _Type { get; set; }

        public ToolType Type
        {
            get => Enum.Parse<ToolType>(_Type);
            set => _Type = value.ToString();
        }

        private string _Status { get; set; }

        public OperationalStatus Status
        {
            get => Enum.Parse<OperationalStatus>(_Status);
            set => _Status = value.ToString();
        }

        public Employee AssignedMaintainer { get; set; }

        public string Brand { get; set; }

        public string ToolName { get; set; }

        public Supplier Supplier { get; set; }
    }

    public enum OperationalStatus
    {
        Malfunctioning,
        Functioning,
        UnderReparation,
        IssueReported,
        IssueInTriage,
    }

    public enum ToolType
    {
        ManufacturingMachine,
        ManufacturingTool,
        PrototypingMachine,
        PrototypingTool
    }
}

#endif