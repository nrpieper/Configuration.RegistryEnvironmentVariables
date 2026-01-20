using Microsoft.Extensions.Configuration;
using System.Collections;

namespace Configuration.RegistryEnvironmentVariables;

/// <summary>
/// A configuration provider that reads environment variables from the Windows Registry.
/// </summary>
/// <remarks>
/// Adapted from source: https://source.dot.net/#Microsoft.Extensions.Configuration.EnvironmentVariables/EnvironmentVariablesConfigurationProvider.cs
/// </remarks>
public class RegistryEnvironmentVariablesConfigurationProvider : ConfigurationProvider
{
    // Connection string prefixes for various services. These prefixes are used to identify connection strings in environment variables.
    // az webapp config connection-string set: https://learn.microsoft.com/en-us/cli/azure/webapp/config/connection-string?view=azure-cli-latest#az-webapp-config-connection-string-set
    // Environment variables and app settings in Azure App Service: https://learn.microsoft.com/en-us/azure/app-service/reference-app-settings?tabs=kudu%2Cdotnet#variable-prefixes
    private const string MySqlServerPrefix = "MYSQLCONNSTR_";
    private const string SqlAzureServerPrefix = "SQLAZURECONNSTR_";
    private const string SqlServerPrefix = "SQLCONNSTR_";
    private const string CustomConnectionStringPrefix = "CUSTOMCONNSTR_";
    private const string PostgreSqlServerPrefix = "POSTGRESQLCONNSTR_";
    private const string ApiHubPrefix = "APIHUBCONNSTR_";
    private const string DocDbPrefix = "DOCDBCONNSTR_";
    private const string EventHubPrefix = "EVENTHUBCONNSTR_";
    private const string NotificationHubPrefix = "NOTIFICATIONHUBCONNSTR_";
    private const string RedisCachePrefix = "REDISCACHECONNSTR_";
    private const string ServiceBusPrefix = "SERVICEBUSCONNSTR_";

    private readonly EnvironmentVariableTarget _environmentVariableTarget;
    private readonly string? _prefix;
    private readonly string _normalizedPrefix;

    /// <summary>
    /// Initializes a new instance of <see cref="RegistryEnvironmentVariablesConfigurationProvider"/>.
    /// </summary>
    /// <param key="environmentVariableTarget">The registry environmentVariableTarget (Machine or User).</param>
    /// <param key="prefix">Optional prefix to filter environment variables.</param>
    public RegistryEnvironmentVariablesConfigurationProvider(EnvironmentVariableTarget environmentVariableTarget, string? prefix = null)
    {
        if (environmentVariableTarget != EnvironmentVariableTarget.Machine && environmentVariableTarget != EnvironmentVariableTarget.User)
        {
            throw new ArgumentException(
                "Only Machine and User targets are supported for registry-based environment variables.",
                nameof(environmentVariableTarget));
        }

        _environmentVariableTarget = environmentVariableTarget;
        _prefix = prefix ?? string.Empty;
        _normalizedPrefix = Normalize(_prefix);
    }

    /// <summary>
    /// Generates a string representing this provider key and relevant details.
    /// </summary>
    /// <returns>The configuration key.</returns>
    public override string ToString()
    {
        string s = GetType().Name;
        if (!string.IsNullOrEmpty(_prefix))
        {
            s += $" Prefix: '{_prefix}'";
        }
        return s;
    }

    /// <summary>
    /// Loads environment variables from the Windows Registry.
    /// </summary>
    public override void Load()
    {
        var data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        try
        {
            var environmentVariables = Environment.GetEnvironmentVariables(_environmentVariableTarget);

            foreach (DictionaryEntry entry in environmentVariables)
            {
                string key = (string)entry.Key;
                string? value = (string?)entry.Value;

                if (key.StartsWith(MySqlServerPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    HandleMatchedConnectionStringPrefix(data, MySqlServerPrefix, "MySql.Data.MySqlClient", key, value);
                }
                else if (key.StartsWith(SqlAzureServerPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    HandleMatchedConnectionStringPrefix(data, SqlAzureServerPrefix, "System.Data.SqlClient", key, value);
                }
                else if (key.StartsWith(SqlServerPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    HandleMatchedConnectionStringPrefix(data, SqlServerPrefix, "System.Data.SqlClient", key, value);
                }
                else if (key.StartsWith(PostgreSqlServerPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    HandleMatchedConnectionStringPrefix(data, PostgreSqlServerPrefix, "Npgsql", key, value);
                }
                else if (key.StartsWith(ApiHubPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    HandleMatchedConnectionStringPrefix(data, ApiHubPrefix, null, key, value);
                }
                else if (key.StartsWith(DocDbPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    HandleMatchedConnectionStringPrefix(data, DocDbPrefix, null, key, value);
                }
                else if (key.StartsWith(EventHubPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    HandleMatchedConnectionStringPrefix(data, EventHubPrefix, null, key, value);
                }
                else if (key.StartsWith(NotificationHubPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    HandleMatchedConnectionStringPrefix(data, NotificationHubPrefix, null, key, value);
                }
                else if (key.StartsWith(RedisCachePrefix, StringComparison.OrdinalIgnoreCase))
                {
                    HandleMatchedConnectionStringPrefix(data, RedisCachePrefix, null, key, value);
                }
                else if (key.StartsWith(ServiceBusPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    HandleMatchedConnectionStringPrefix(data, ServiceBusPrefix, null, key, value);
                }
                else if (key.StartsWith(CustomConnectionStringPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    HandleMatchedConnectionStringPrefix(data, CustomConnectionStringPrefix, null, key, value);
                }
                else
                {
                    AddIfNormalizedKeyMatchesPrefix(data, Normalize(key), value);
                }
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Failed to read environment variables from registry environmentVariableTarget '{_environmentVariableTarget}'.", ex);
        }

        Data = data;
    }

    private void HandleMatchedConnectionStringPrefix(Dictionary<string, string?> data, string connectionStringPrefix, string? provider, string fullKey, string? value)
    {
        string normalizedKeyWithoutConnectionStringPrefix = Normalize(fullKey.Substring(connectionStringPrefix.Length));

        // Add the key-value pair for connection string, and optionally provider key
        AddIfNormalizedKeyMatchesPrefix(data, $"ConnectionStrings:{normalizedKeyWithoutConnectionStringPrefix}", value);
        if (provider != null)
        {
            AddIfNormalizedKeyMatchesPrefix(data, $"ConnectionStrings:{normalizedKeyWithoutConnectionStringPrefix}_ProviderName", provider);
        }
    }

    private void AddIfNormalizedKeyMatchesPrefix(Dictionary<string, string?> data, string normalizedKey, string? value)
    {
        if (normalizedKey.StartsWith(_normalizedPrefix, StringComparison.OrdinalIgnoreCase))
        {
            data[normalizedKey.Substring(_normalizedPrefix.Length)] = value;
        }
    }

    private static string Normalize(string key) => key.Replace("__", ConfigurationPath.KeyDelimiter);
}
