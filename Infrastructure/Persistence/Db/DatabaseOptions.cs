namespace Infrastructure.Persistence.Db;

public class DatabaseOptions
{
    public string ConnectionString { get; set; } = string.Empty;
    public string? DatabaseType { get; set; }
}
