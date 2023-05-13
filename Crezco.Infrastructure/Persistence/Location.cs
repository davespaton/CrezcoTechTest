using Crezco.Infrastructure.External.LocationClient;
using ProtoBuf;

namespace Crezco.Infrastructure.Persistence;

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public class Location
{
    public required string IpAddress { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required LocationApiData Data { get; init; }
}