using MongoDB.Bson;

namespace ObjectsAsAPI.Models;

public interface IRequest<P, R>
    where P: IPayload
    where R: IResponse
{
    public ObjectId Id { get; set; }

    public string CreatorId { get; set; }

    public RequestStatus Status { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public P? Payload { get; set; }

    public R? Response { get; set; }
}

public interface IPayload
{
}

public interface IResponse
{
    public ResponseStatus Status { get; }

    // Filled if Status == Rejected
    public string? RejectedReason { get; }
}

public enum RequestStatus
{
    Draft, // The user is still editing, it shouldn't be handled
    Pending, // User confirmed, waiting for backend
    Handled, // Response created
}

public enum ResponseStatus
{
    Approved,
    Rejected,
}