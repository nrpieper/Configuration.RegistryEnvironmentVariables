# Registry Environment Variables Configuration Provider

A custom configuration provider for ASP.NET Core that reads environment variables directly from the Windows Registry instead of the current process environment.

## Overview

By default, ASP.NET Core's `AddEnvironmentVariables()` reads environment variables from the current process context. 
This custom provider reads environment variables directly from the Windows Registry, 
allowing you to access Machine or User-level environment variables that may not be loaded into the current process.

Background and other options: [Setting application environment variables in IIS without restarts](https://andrewlock.net/setting-environment-variables-in-iis-and-avoiding-app-pool-restarts/)

## Features

- Read environment variables from Registry (Machine or User scope)
- Optional prefix filtering to load only specific variables
- Automatic hierarchical key conversion (double underscores `__` become `:` for nested configuration)
- Reusable across multiple projects
- Works with .NET 9 and compatible with earlier versions

## Usage

### Basic Usage

Add registry environment variables from the Machine scope:

in `program.cs`:
```csharp
using Configuration.RegistryEnvironmentVariables;

var builder = WebApplication.CreateBuilder(args);

// Add Machine-level registry environment variables
builder.Configuration.AddRegistryEnvironmentVariables(EnvironmentVariableTarget.Machine);
```

### User Scope

Read from the User-level registry:

```csharp
builder.Configuration.AddRegistryEnvironmentVariables(EnvironmentVariableTarget.User);
```

### With Prefix Filter

Load only environment variables that start with a specific prefix:

```csharp
// Only loads variables starting with "MYAPP_"
// The prefix is removed from the configuration key
builder.Configuration.AddRegistryEnvironmentVariables("MYAPP_", EnvironmentVariableTarget.Machine);
```

### Advanced Configuration

Use the action-based overload for complete control:

```csharp
builder.Configuration.AddRegistryEnvironmentVariables(source =>
{
    source.Target = EnvironmentVariableTarget.User;
    source.Prefix = "MYAPP_";
});
```

## Hierarchical Configuration

The provider automatically converts double underscores (`__`) to colons (`:`) for hierarchical configuration keys:

**Registry Variable:**
```
MYAPP__Database__ConnectionString = "Server=localhost;..."
```

**Configuration Key (after removing "MYAPP_" prefix):**
```csharp
var connectionString = builder.Configuration["Database:ConnectionString"];
```

## Registry Locations

- **Machine**: `HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Environment`
- **User**: `HKEY_CURRENT_USER\Environment`

## Reusability

To reuse in other projects, copy the `Configuration.RegistryEnvironmentVariables` folder containing:
- `RegistryEnvironmentVariablesConfigurationSource.cs`
- `RegistryEnvironmentVariablesConfigurationProvider.cs`
- `RegistryEnvironmentVariablesExtensions.cs`

Then add the using statement and call the extension method in your `Program.cs` or startup configuration.

## Requirements

- Windows operating system (uses Windows Registry)
- .NET 6.0 or later
- Appropriate permissions to read from the registry

## Example Scenarios

### Scenario 1: Reading Database Connection Strings

Set a machine-level environment variable:
```
MYAPP__ConnectionStrings__DefaultConnection = "Server=myserver;Database=mydb;..."
```

In your code:
```csharp
builder.Configuration.AddRegistryEnvironmentVariables("MYAPP_", EnvironmentVariableTarget.Machine);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
```

### Scenario 2: Application Settings

Set user-level environment variables:
```
AppName = "MyApplication"
AppVersion = "1.0.0"
```

In your code:
```csharp
builder.Configuration.AddRegistryEnvironmentVariables(EnvironmentVariableTarget.User);
var appName = builder.Configuration["AppName"];
var appVersion = builder.Configuration["AppVersion"];
```

## Configuration Provider Order

When you call `WebApplication.CreateBuilder(args)`, ASP.NET Core automatically sets up configuration providers in this order:

1. **`appsettings.json`**
2. **`appsettings.{Environment}.json`** (e.g., `appsettings.Development.json`)
3. **User Secrets** (in Development environment only)
4. **Environment Variables** (from current process)
5. **Command-line arguments**

When you add the registry environment variables provider after creating the builder:

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddRegistryEnvironmentVariables(EnvironmentVariableTarget.Machine);
```

The final order becomes:

1. `appsettings.json`
2. `appsettings.{Environment}.json`
3. User Secrets (Development only)
4. Environment Variables (process)
5. Command-line arguments
6. **Registry Environment Variables** ? Added last

### What This Means

**The registry environment variables provider is added last and will override all previous configuration sources.**

Configuration providers follow a "last one wins" principle - if the same configuration key exists in multiple sources, the value from the last provider in the chain is used.

With this setup:
- Registry environment variables **override** values from `appsettings.json`, process environment variables, and command-line arguments
- This gives registry-based configuration the highest priority
- Useful for system-wide settings that should always take precedence

If you need a different priority order, you can manipulate the `builder.Configuration.Sources` collection to insert the provider at a specific position before building the application.

## Notes

- The provider reads registry values at application startup
- Changes to registry environment variables require application restart to take effect
- Only `EnvironmentVariableTarget.Machine` and `EnvironmentVariableTarget.User` are supported
- The `EnvironmentVariableTarget.Process` target is not supported (use default `AddEnvironmentVariables()` instead)
- Registry environment variables are added last in the configuration chain and will override all other sources

