using MongoDB.Bson;
using Realms;

namespace ObjectsAsAPI.Models;

//TODO Putting the setter as private to "simulate" readonly-ness
public partial class Order : IRealmObject
{
    [PrimaryKey]
    [MapTo("_id")]
    public ObjectId Id { get; private set; }

    [MapTo("_creatorId")]
    public string CreatorId { get; private set; } = null!;

    public OrderContent? Content { get; private set; }

    private string _Status { get; set; } = null!;

    public OrderStatus Status
    {
        get => Enum.Parse<OrderStatus>(_Status);
        private set => _Status = value.ToString();
    }
}

public partial class OrderContent : IEmbeddedObject
{
    public DateTimeOffset CreatedAt { get; set; }

    public IList<OrderItem> Items { get; } = null!;

    public string? OrderName { get; set; }

    public OrderContent()
    {
        CreatedAt = DateTimeOffset.Now;
    }
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

