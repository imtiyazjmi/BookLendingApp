using BookLendingApp.Data;
using BookLendingApp.Services;
using System.Diagnostics.CodeAnalysis;

namespace BookLendingApp.Services;

[ExcludeFromCodeCoverage]
public class DatabaseConfigurationService
{
    private readonly IConfiguration _configuration;
    private readonly ParameterStoreService? _parameterStoreService;

    public DatabaseConfigurationService(IConfiguration configuration, ParameterStoreService? parameterStoreService = null)
    {
        _configuration = configuration;
        _parameterStoreService = parameterStoreService;
    }

    public async Task<string> GetConnectionStringAsync()
    {
        var useSSM = _configuration.GetValue<bool>("UseSSM");
        
        string host, database, username, password;

        if (useSSM && _parameterStoreService != null)
        {
            var ssmSettings = _configuration.GetSection("AWS:SSM").Get<SsmSettings>()!;
            host = await _parameterStoreService.GetParameterAsync(ssmSettings.PostgreSqlHostParam);
            database = await _parameterStoreService.GetParameterAsync(ssmSettings.PostgreSqlDatabaseParam);
            username = await _parameterStoreService.GetParameterAsync(ssmSettings.PostgreSqlUsernameParam);
            password = await _parameterStoreService.GetParameterAsync(ssmSettings.PostgreSqlPasswordParam);
        }
        else
        {
            var pgSettings = _configuration.GetSection("PostgreSQL").Get<PostgreSqlSettings>()!;
            host = pgSettings.Host;
            database = pgSettings.Database;
            username = pgSettings.Username;
            password = pgSettings.Password;
        }

        return $"Host={host};Database={database};Username={username};Password={password}";
    }
}