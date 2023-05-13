using ProtoBuf;

namespace Crezco.Infrastructure.Cache;

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
internal sealed class CacheWrapper<T> where T : class
{
    public T? Value { get; set; }

    // Required for ProtoBuf
    private CacheWrapper() { }

    public CacheWrapper(T? value)
    {
        Value = value;
    }

    public byte[] ToByteArray()
    {
        using var memoryStream = new MemoryStream();
        Serializer.Serialize(memoryStream, this);
        return memoryStream.ToArray();
    }

    public static CacheWrapper<T> FromByteArray(byte[] data) 
    {
        using var memoryStream = new MemoryStream(data);
        return Serializer.Deserialize<CacheWrapper<T>>(memoryStream);
    }
}