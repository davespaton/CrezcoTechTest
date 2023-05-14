namespace Crezco.Infrastructure.Persistence;

public class DatabaseOptions
{
    public const string SectionName = "Database";

    public required string ConnectionString { get; set; }
    public required string DatabaseName { get; set; }
}