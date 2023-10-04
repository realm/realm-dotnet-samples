using MongoDB.Bson;
using ObjectsAsAPI.Services;
using Realms;

namespace ObjectsAsAPI.Models;

public partial class CreateOrderRequest : IRealmObject, IRequest<CreateOrderPayload, CreateOrderResponse>
{
    [PrimaryKey]
    [MapTo("_id")]
    public ObjectId Id { get; private set; } = ObjectId.GenerateNewId();

    [MapTo("_creatorId")]
    public string CreatorId { get; private set; }

    [MapTo("status")]
    private string _Status { get; set; } = RequestStatus.Draft.ToString();

    public RequestStatus Status
    {
        get => Enum.Parse<RequestStatus>(_Status);
        set => _Status = value.ToString();
    }

    [MapTo("createdAt")]
    public DateTimeOffset CreatedAt { get; private set; }

    [MapTo("payload")]
    public CreateOrderPayload? Payload { get; set; }

    [MapTo("response")]
    public CreateOrderResponse? Response { get; set; }

    [MapTo("rejectedReason")]
    public string? RejectedReason { get; private set; }

    // Used in the UI
    public string? Description
    {
        get
        {
            var requestType = "CreateOrder";
            var orderIdentifier = Payload?.Content?.OrderName;

            if (orderIdentifier?.Length > 10)
            {
                orderIdentifier = orderIdentifier[..10];
            }

            var status = Status switch
            {
                RequestStatus.Approved => "✅ ",
                RequestStatus.Rejected => "❌ ",
                _ => string.Empty,
            };

            return $"{status}{requestType}{(string.IsNullOrEmpty(orderIdentifier) ? "" : $" - {orderIdentifier}")}";
        }
    }

    // Used in the UI
    public string? StatusString
    {
        get
        {
            return Status switch
            {
                RequestStatus.Draft => "Draft",
                RequestStatus.Pending => "Pending",
                RequestStatus.Approved => "Approved",
                RequestStatus.Rejected => $"Rejected{(string.IsNullOrEmpty(RejectedReason) ? "" : $": \"{RejectedReason}\"")}",
                _ => throw new NotImplementedException(),
            };
        }
    }

    public CreateOrderRequest()
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
        if (propertyName == nameof(_Status))
        {
            RaisePropertyChanged(nameof(Status));
            RaisePropertyChanged(nameof(StatusString));
            RaisePropertyChanged(nameof(Description));
        }
    }
}

public partial class CreateOrderPayload : IEmbeddedObject, IPayload
{
    [MapTo("content")]
    public OrderContent? Content { get; set; }
}

public partial class CreateOrderResponse : IEmbeddedObject, IResponse
{
    [MapTo("order")]
    public Order? Order { get; private set; }
}

