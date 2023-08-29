using MongoDB.Bson;
using ObjectsAsAPI.Services;
using Realms;

namespace ObjectsAsAPI.Models;

public partial class CancelOrderPayload : IRealmObject, IPayload
{
    [PrimaryKey]
    [MapTo("_id")]
    public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

    [MapTo("_creatorId")]
    public string CreatorId { get; set; }

    [MapTo("orderId")]
    public ObjectId OrderId { get; set; }

    public CancelOrderPayload()
    {
        if (RealmService.CurrentUser == null)
        {
            throw new Exception("Login before using models!");
        }

        CreatorId = RealmService.CurrentUser.Id;
    }
}

public partial class CancelOrderResponse : IRealmObject, IResponse
{
    [PrimaryKey]
    [MapTo("_id")]
    public ObjectId Id { get; private set; }

    [MapTo("_creatorId")]
    public string CreatorId { get; private set; } = null!;

    [MapTo("orderId")]
    public ObjectId OrderId { get; set; }

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
