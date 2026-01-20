using Microsoft.Extensions.Configuration;

namespace Configuration.RegistryEnvironmentVariables;

/// <summary>
/// Represents registry environment variables as an <see cref="IConfigurationSource"/>.
/// </summary>
public class RegistryEnvironmentVariablesConfigurationSource : IConfigurationSource
{
    /// <summary>
    /// Gets or sets the target scope for registry environment variables.
    /// </summary>
    public EnvironmentVariableTarget Target { get; set; } = EnvironmentVariableTarget.Machine;

    /// <summary>
    /// Gets or sets a prefix used to filter environment variables.
    /// </summary>
    public string? Prefix { get; set; }

    /// <summary>
    /// Builds the <see cref="RegistryEnvironmentVariablesConfigurationProvider"/> for this source.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigurationBuilder"/>.</param>
    /// <returns>A <see cref="RegistryEnvironmentVariablesConfigurationProvider"/></returns>
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new RegistryEnvironmentVariablesConfigurationProvider(Target, Prefix);
    }
}
