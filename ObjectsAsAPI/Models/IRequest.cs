using MongoDB.Bson;

namespace ObjectsAsAPI.Models;

public interface IRequest<P, R>
    where P: IPayload
    where R: IResponse
{
    public ObjectId Id { get; }

    public string CreatorId { get; }

    public DateTimeOffset CreatedAt { get; }

    public RequestStatus Status { get; set; }

    public string? RejectedReason { get; }

    public P? Payload { get; set; }

    public R? Response { get; set; }
}

public interface IPayload
{
}

public interface IResponse
{
}

public enum RequestStatus
{
    Draft, // The user is still editing, it shouldn't be handled
    Pending, // User confirmed, waiting for backend
    Approved, // Request handled, approved
    Rejected, // Request handled, rejected
}