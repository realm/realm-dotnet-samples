using MongoDB.Bson;
using ObjectsAsAPI.Services;
using Realms;

namespace ObjectsAsAPI.Models;

public partial class CreateOrderRequest : IRealmObject, IRequest<CreateOrderPayload, CreateOrderResponse>
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
    public CreateOrderPayload? Payload { get; set; }

    [MapTo("response")]
    public CreateOrderResponse? Response { get; set; }

    // Used in the UI
    public string? Description
    {
        get
        {
            //TODO Check if all correct
            string? status = null;

            var requestType = "CreateOrder";
            var orderIdentifier = Payload?.Content?.OrderName;

            if (orderIdentifier?.Length > 10)
            {
                orderIdentifier = orderIdentifier[..10];
            }

            if (Response != RealmValue.Null)
            {
                status = Response?.Status switch
                {
                    ResponseStatus.Approved => "✅ ",
                    ResponseStatus.Rejected => "❌ ",
                    _ => throw new NotImplementedException(),
                };
            }

            return $"{status}{requestType}{(string.IsNullOrEmpty(orderIdentifier) ? "" : $" - {orderIdentifier}")}";
        }
    }

    // Used in the UI
    public string? StatusString
    {
        get
        {
            //TODO CHeck if needed
            return Status switch
            {
                RequestStatus.Draft => "Draft",
                RequestStatus.Pending => "Pending",
                RequestStatus.Handled => $"{Response?.Status}{(string.IsNullOrEmpty(Response?.RejectedReason) ? "" : $": \"{Response?.RejectedReason}\"")}",
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
        //TODO Check if needed
        if (propertyName == nameof(_Status))
        {
            RaisePropertyChanged(nameof(Status));
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

