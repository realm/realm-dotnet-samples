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

    [MapTo("status")]
    private string _Status { get; set; } = null!;

    public RequestStatus Status
    {
        get => Enum.Parse<RequestStatus>(_Status);
        set => _Status = value.ToString();
    }

    [MapTo("createdAt")]
    public DateTimeOffset CreatedAt { get; set; }

    [MapTo("payload")]
    public RealmValue Payload { get; set; }

    [MapTo("response")]
    public RealmValue Response { get; set; }

    // Used in the UI
    public string? Description
    {
        get
        {
            var payloadClass = Payload.AsIRealmObject().ObjectSchema?.Name;

            string? status = null;
            string? requestType = null;
            string? orderName = null;

            if (payloadClass == nameof(CreateOrderPayload))
            {
                requestType = "CreateOrder";
                orderName = Payload.AsRealmObject<CreateOrderPayload>()?.Content?.OrderName;

                if (Response != RealmValue.Null)
                {
                    var response = Response.AsRealmObject<CreateOrderResponse>();

                    status = response.Status switch
                    {
                        ResponseStatus.Approved => "✅ ",
                        ResponseStatus.Rejected => "❌ ",
                        _ => throw new NotImplementedException(),
                    };
                }
            }

            return $"{status}{requestType} - {orderName}";
        }
    }

    public AtlasRequest()
    {
        if (RealmService.CurrentUser == null)
        {
            throw new Exception("Login before using models!");
        }

        CreatorId = RealmService.CurrentUser.Id;
        CreatedAt = DateTimeOffset.Now;
    }

    partial void OnPropertyChanged(string? propertyName)
    {
        //TODO Can we do something so that we raise a notification when the Payload changes?
        if (propertyName == nameof(_Status))
        {
            RaisePropertyChanged(nameof(Status));
            RaisePropertyChanged(nameof(Description));
        }
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
    public string CreatorId { get; set; }

    public ObjectId Id { get; set; }
}

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

public partial class CreateOrderPayload : IRealmObject, IPayload
{
    [PrimaryKey]
    [MapTo("_id")]
    public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

    [MapTo("_creatorId")]
    public string CreatorId { get; set; }

    [MapTo("content")]
    public OrderContent? Content { get; set; }

    public CreateOrderPayload()
    {
        if (RealmService.CurrentUser == null)
        {
            throw new Exception("Login before using models!");
        }

        CreatorId = RealmService.CurrentUser.Id;
    }
}

public partial class CreateOrderResponse : IRealmObject, IResponse
{
    [PrimaryKey]
    [MapTo("_id")]
    public ObjectId Id { get; private set; }

    [MapTo("_creatorId")]
    public string CreatorId { get; private set; } = null!;

    [MapTo("order")]
    public Order? Order { get; private set; }

    [MapTo("status")]
    private string _Status { get; set; } = null!;

    public ResponseStatus Status
    {
        get => Enum.Parse<ResponseStatus>(_Status);
        private set => _Status = value.ToString();
    }

    [MapTo("rejectedReason")]
    public string? RejectedReason { get; private set; }
}

/* What I don't like about generic request class:
 *  - Payload/Response can't be embedded, so they need to have an ID and CreatorID
 *  - I have a feeling creating the actions will be more complex, because I need to create other objects and then set them
 * 
 * 
 * What I don't like about specific request class:
 *  - I'm not sure how to show them in the UI (we would need to separate them probably)
 *  - If I use a trigger on modifications, it will be called all the time when creating the payload
 * 
 * 
 */
