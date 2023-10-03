﻿using MongoDB.Bson;
using ObjectsAsAPI.Services;
using Realms;

namespace ObjectsAsAPI.Models;

public partial class CancelOrderRequest : IRealmObject, IRequest<CancelOrderPayload, CancelOrderResponse>
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
    public CancelOrderPayload? Payload { get; set; }

    [MapTo("response")]
    public CancelOrderResponse? Response { get; set; }

    // Used in the UI
    public string? Description
    {
        get
        {
            string? status = null;

            var requestType = "CancelOrder";
            var orderIdentifier = Payload?.OrderId.ToString();

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
            return Status switch
            {
                RequestStatus.Draft => "Draft",
                RequestStatus.Pending => "Pending",
                RequestStatus.Handled => $"{Response?.Status}{(string.IsNullOrEmpty(Response?.RejectedReason) ? "" : $": \"{Response?.RejectedReason}\"")}",
                _ => throw new NotImplementedException(),
            };
        }
    }

    public CancelOrderRequest()
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

public partial class CancelOrderPayload : IEmbeddedObject, IPayload
{
    [MapTo("orderId")]
    public ObjectId OrderId { get; set; }
}

public partial class CancelOrderResponse : IEmbeddedObject, IResponse
{
    [MapTo("orderId")]
    public ObjectId OrderId { get; set; }

    [MapTo("status")]
    private string _Status { get; set; } = ResponseStatus.Approved.ToString();

    public ResponseStatus Status
    {
        get => Enum.Parse<ResponseStatus>(_Status);
        private set => _Status = value.ToString();
    }

    [MapTo("rejectedReason")]
    public string? RejectedReason { get; private set; }
}
