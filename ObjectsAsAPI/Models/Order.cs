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

    [MapTo("content")]
    public OrderContent? Content { get; private set; }

    [MapTo("status")]
    private string _Status { get; set; } = null!;

    public OrderStatus Status
    {
        get => Enum.Parse<OrderStatus>(_Status);
        private set => _Status = value.ToString();
    }
}

public partial class OrderContent : IEmbeddedObject
{
    [MapTo("createdAt")]
    public DateTimeOffset CreatedAt { get; set; }

    [MapTo("items")]
    public IList<OrderItem> Items { get; } = null!;

    [MapTo("orderName")]
    public string? OrderName { get; set; }

    public OrderContent()
    {
        CreatedAt = DateTimeOffset.Now;
    }
}

public partial class OrderItem : IEmbeddedObject
{
    [MapTo("itemName")]
    public string? ItemName { get; set; }

    [MapTo("itemQuantity")]
    public int ItemQuantity { get; set; }
}

public enum OrderStatus
{
    Approved,
    Fulfilled,
    Cancelled,
}

