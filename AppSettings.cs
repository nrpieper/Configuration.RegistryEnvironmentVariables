using Configuration.RegistryEnvironmentVariables;

namespace APITestLoadingRegEnvVar
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Bound from builder.Configuration.Get/<AppSettings/>()
    /// Properties are bound in this order: (ref: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-10.0#default-application-configuration-sources)
    /// 1. Command-line
    /// 2. Environment variables (non-prefixed Not DOTNET_ or ASPCORE_)
    /// 3. UserSecrets when app runs in the Development environment (APSCORE_ENVIRONMENT=Development)
    /// 4. appsettings.{Environment}.json
    /// 5. appsettings.json
    /// </remarks>
    public class AppSettings
    {
        public string? Property1 { get; init; }
        public required AppSettingsNestedTest NestedTest { get; init; }
    }

    public class AppSettingsNestedTest
    {
        public string? NestedProperty1 { get; init; }
        public string? NestedProperty2 { get; init; }
    }
}
