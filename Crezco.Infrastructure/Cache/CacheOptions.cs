﻿namespace Crezco.Infrastructure.Cache;
public sealed class CacheOptions
{
    public const string SectionName = "Cache";

    public bool Disabled { get; set; }
    public required string Configuration { get; set; }
    public required string InstanceName { get; set; }
}
