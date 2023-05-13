using Crezco.Infrastructure.External.LocationClient;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

namespace Crezco.Infrastructure.Persistence;

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public class Location
{
    [BsonId]
    public ObjectId Id { get; set; }

    public required string IpAddress { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required LocationApiData Data { get; init; }
}