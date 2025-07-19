using Microsoft.Extensions.DependencyInjection;

namespace Core.Services;

/// <summary>
/// Service locator for accessing dependency injection services from static contexts
/// This provides backward compatibility while transitioning to full dependency injection
/// </summary>
public static class ServiceProvider
{
    private static IServiceProvider? _serviceProvider;

    /// <summary>
    /// Configure the service provider (called during application startup)
    /// </summary>
    /// <param name="serviceProvider">The configured service provider</param>
    public static void Configure(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Get a service of type T
    /// </summary>
    /// <typeparam name="T">Service type</typeparam>
    /// <returns>Service instance</returns>
    /// <exception cref="InvalidOperationException">Thrown if service provider is not configured</exception>
    public static T GetService<T>() where T : notnull
    {
        if (_serviceProvider == null)
        {
            throw new InvalidOperationException("Service provider has not been configured. Call Configure() during application startup.");
        }

        return _serviceProvider.GetRequiredService<T>();
    }

    /// <summary>
    /// Try to get a service of type T
    /// </summary>
    /// <typeparam name="T">Service type</typeparam>
    /// <returns>Service instance or null if not found or not configured</returns>
    public static T? GetServiceOrDefault<T>() where T : class
    {
        return _serviceProvider?.GetService<T>();
    }
}