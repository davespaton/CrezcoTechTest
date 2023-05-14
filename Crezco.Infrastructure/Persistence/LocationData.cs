using Crezco.Infrastructure.External.LocationClient;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using ProtoBuf;

namespace Crezco.Infrastructure.Persistence;

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public class LocationData
{
    [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
    public string Id { get; set; }

    public required string IpAddress { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required LocationApiData Data { get; init; }
}