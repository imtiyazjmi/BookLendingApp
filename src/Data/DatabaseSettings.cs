using System.Diagnostics.CodeAnalysis;

namespace BookLendingApp.Data;

[ExcludeFromCodeCoverage]
public class PostgreSqlSettings
{
    public string Host { get; set; } = string.Empty;
    public string Database { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

[ExcludeFromCodeCoverage]
public class AwsSettings
{
    public string Region { get; set; } = string.Empty;
    public SsmSettings SSM { get; set; } = new();
}

[ExcludeFromCodeCoverage]
public class SsmSettings
{
    public string PostgreSqlHostParam { get; set; } = string.Empty;
    public string PostgreSqlDatabaseParam { get; set; } = string.Empty;
    public string PostgreSqlUsernameParam { get; set; } = string.Empty;
    public string PostgreSqlPasswordParam { get; set; } = string.Empty;
}