namespace Crezco.Infrastructure.Cache.Helpers;

//[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
//public class CacheItem
//{
//    public byte[] ToByteArray()
//    {
//        using var memoryStream = new MemoryStream();
//        Serializer.Serialize(memoryStream, this);
//        return memoryStream.ToArray();
//    }

//    public static T FromByteArray<T>(byte[] data) where T : CacheItem
//    {
//        using var memoryStream = new MemoryStream(data);
//        return Serializer.Deserialize<T>(memoryStream);
//    }
//}