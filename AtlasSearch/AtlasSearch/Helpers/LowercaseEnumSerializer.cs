using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace AtlasSearch.Helpers;

internal class LowercaseEnumSerializer<T> : SerializerBase<T>
    where T : struct
{
    public override T Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var type = context.Reader.GetCurrentBsonType();
        if (type != BsonType.String)
        {
            throw CreateCannotDeserializeFromBsonTypeException(type);
        }

        var value = context.Reader.ReadString();
        if (Enum.TryParse<T>(value, ignoreCase: true, out var result))
        {
            return result;
        }

        throw new NotSupportedException($"Unexpected representation string for HiglightType: {value}");
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, T value)
    {
        context.Writer.WriteString(value.ToString()?.ToLowerInvariant());
    }
}
