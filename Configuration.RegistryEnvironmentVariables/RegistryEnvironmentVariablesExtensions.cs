using Microsoft.Extensions.Configuration;

namespace Configuration.RegistryEnvironmentVariables;

/// <summary>
/// Extension methods for registering <see cref="RegistryEnvironmentVariablesConfigurationProvider"/> with <see cref="IConfigurationBuilder"/>.
/// </summary>
public static class RegistryEnvironmentVariablesExtensions
{
    /// <summary>
    /// Adds environment variables from the Windows Registry to the configuration builder.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
    /// <param name="target">The registry target (Machine or User). Defaults to Machine.</param>
    /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
    public static IConfigurationBuilder AddRegistryEnvironmentVariables(
        this IConfigurationBuilder builder,
        EnvironmentVariableTarget target = EnvironmentVariableTarget.Machine)
    {
        return builder.Add(new RegistryEnvironmentVariablesConfigurationSource
        {
            Target = target
        });
    }

    /// <summary>
    /// Adds environment variables from the Windows Registry to the configuration builder with a prefix filter.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
    /// <param name="prefix">The prefix used to filter environment variables. Only variables starting with this prefix will be included.</param>
    /// <param name="target">The registry target (Machine or User). Defaults to Machine.</param>
    /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
    public static IConfigurationBuilder AddRegistryEnvironmentVariables(
        this IConfigurationBuilder builder,
        string prefix,
        EnvironmentVariableTarget target = EnvironmentVariableTarget.Machine)
    {
        return builder.Add(new RegistryEnvironmentVariablesConfigurationSource
        {
            Target = target,
            Prefix = prefix
        });
    }

    /// <summary>
    /// Adds environment variables from the Windows Registry to the configuration builder with custom configuration.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
    /// <param name="configureSource">Configures the source.</param>
    /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
    public static IConfigurationBuilder AddRegistryEnvironmentVariables(
        this IConfigurationBuilder builder,
        Action<RegistryEnvironmentVariablesConfigurationSource> configureSource)
    {
        var source = new RegistryEnvironmentVariablesConfigurationSource();
        configureSource(source);
        return builder.Add(source);
    }
}
