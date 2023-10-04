using MongoDB.Bson;

namespace ObjectsAsAPI.Models;

public interface IRequest
{
    public ObjectId Id { get; }

    public string CreatorId { get; }

    public DateTimeOffset CreatedAt { get; }

    public RequestStatus Status { get; set; }

    public string? RejectedReason { get; }
}

public enum RequestStatus
{
    Draft, // The user is still editing, it shouldn't be handled
    Pending, // User confirmed, waiting for backend
    Approved, // Request handled, approved
    Rejected, // Request handled, rejected
}