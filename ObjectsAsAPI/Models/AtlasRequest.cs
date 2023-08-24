using MongoDB.Bson;
using Realms;

namespace ObjectsAsAPI.Models;

public partial class AtlasRequest : IRealmObject
{
    public ObjectId CreatorId { get; }

    public ObjectId RequestId { get; set; } = ObjectId.GenerateNewId();

    private string _Status { get; set; } = null!; //TODO Need to fix nullability

    public RequestStatus Status
    {
        get => Enum.Parse<RequestStatus>(_Status);
        set => _Status = value.ToString();
    }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset HandledAt { get; set; }

    public RealmValue Payload { get; set; }

    public RealmValue Response { get; set; }
}

public enum RequestStatus
{
    Draft, // The user is still editing, it shouldn't be handled
    Pending, // User confirmed, waiting for backend
    Handled, // Response created
}

public interface IPayload
{
    // ???
}

public interface IResponse
{
    public ResponseStatus Status { get; set; }

    // Filled if Status == Rejected
    public string? RejectedReason { get; set; }
}

// Enum representing if the request has been approved on not, not sure about the name
// RequestStatus enum already exists
public enum ResponseStatus
{
    // We can mention that this could have more states, like "PartiallyApproved"
    Approved,
    Rejected,
}

public partial class CreatedOrderRequestPayload : IRealmObject, IPayload
{
    public OrderContent? Content { get; set; }
}

public partial class CreateOrderRequestResponse : IRealmObject, IResponse
{
    public Order? Order { get; set; }

    private string _Status { get; set; } = null!; //TODO Need to fix nullability

    public ResponseStatus Status
    {
        get => Enum.Parse<ResponseStatus>(_Status);
        set => _Status = value.ToString();
    }

    public string? RejectedReason { get; set; }
}
