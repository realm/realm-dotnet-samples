using MongoDB.Bson;
using ObjectsAsAPI.Services;
using Realms;

namespace ObjectsAsAPI.Models;

public partial class AtlasRequest : IRealmObject
{
    [PrimaryKey]
    [MapTo("_id")]
    public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

    [MapTo("_creatorId")]
    public string CreatorId { get; set; }

    private string _Status { get; set; } = null!;

    public RequestStatus Status
    {
        get => Enum.Parse<RequestStatus>(_Status);
        set => _Status = value.ToString();
    }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset HandledAt { get; set; }

    public RealmValue Payload { get; set; }

    public RealmValue Response { get; set; }

    public AtlasRequest()
    {
        if (RealmService.CurrentUser == null)  //TODO This can go in a method
        {
            throw new Exception("Login before using models!");
        }

        CreatorId = RealmService.CurrentUser.Id;
        CreatedAt = DateTimeOffset.Now;
    }
}

public enum RequestStatus
{
    Draft, // The user is still editing, it shouldn't be handled
    Pending, // User confirmed, waiting for backend
    Handled, // Response created
}

public interface IPayload
{
    //IF these were embedded we wouldn't need those two
    public string CreatorId { get; set; }

    public ObjectId Id { get; set; }
}

//TODO Removing the setter as responses should be read-only
/// but adding the setter as private in the concrete class
/// otherwise the property doesn't get picked up by trealm
public interface IResponse
{
    public string CreatorId { get; }

    public ObjectId Id { get; }

    public ResponseStatus Status { get; }

    // Filled if Status == Rejected
    public string? RejectedReason { get; }
}

public enum ResponseStatus
{
    Approved,
    Rejected,
}

public partial class CreatedOrderRequestPayload : IRealmObject, IPayload
{
    [PrimaryKey]
    [MapTo("_id")]
    public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

    [MapTo("_creatorId")]
    public string CreatorId { get; set; }

    public OrderContent? Content { get; set; }

    public CreatedOrderRequestPayload()
    {
        if (RealmService.CurrentUser == null)
        {
            throw new Exception("Login before using models!");
        }

        CreatorId = RealmService.CurrentUser.Id;
    }
}

public partial class CreateOrderRequestResponse : IRealmObject, IResponse
{
    [PrimaryKey]
    [MapTo("_id")]
    public ObjectId Id { get; private set; }

    [MapTo("_creatorId")]
    public string CreatorId { get; private set; } = null!;

    public Order? Order { get; private set; }

    private string _Status { get; set; } = null!;

    public ResponseStatus Status
    {
        get => Enum.Parse<ResponseStatus>(_Status);
        private set => _Status = value.ToString();
    }

    public string? RejectedReason { get; private set; }
}

/* What I don't like about generic request class:
 *  - Payload/Response can't be embedded, so they need to have an ID and CreatorID
 *  - I have a feeling creating the actions will be more complex, because I need to create other objects and then set them
 * 
 * 
 * What I don't like about specific request class:
 *  - I'm not sure how to show them in the UI (we would need to separate them probably)
 * 
 * 
 */
