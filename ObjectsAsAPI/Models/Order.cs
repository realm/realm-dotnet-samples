using MongoDB.Bson;
using Realms;

namespace ObjectsAsAPI.Models;

public partial class Order : IRealmObject
{
    public ObjectId CreatorId { get; set; }

    public ObjectId OrderId { get; set; }

    public OrderContent? Content { get; set; }

    private string _Status { get; set; } = null!; //TODO Need to fix nullability

    public OrderStatus Status
    {
        get => Enum.Parse<OrderStatus>(_Status);
        set => _Status = value.ToString();
    }

    public bool RequestedChange { get; set; }
}

public partial class OrderContent : IEmbeddedObject
{
    public DateTimeOffset CreatedAt { get; set; }

    public IList<OrderItem> Items { get; } = null!; //TODO Need to fix nullability

    public string? OrderName { get; set; }
}

public partial class OrderItem : IEmbeddedObject
{
    public string? ItemName { get; set; }

    public int ItemQuantity { get; set; }
}

public enum OrderStatus
{
    Approved,
    Processing, // Could be removed
    Fulfilled,
    Cancelled,
    Rejected,
}

