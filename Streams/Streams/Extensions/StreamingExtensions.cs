using Microsoft.Extensions.DependencyInjection;
using Orleans;
using Streams.Models;
using Streams.Models.Interfaces;
using Streams.Models.MessageBus;

namespace Streams.Extensions;

public static class StreamingExtensions
{
    public static void AddArtisStreaming(this IServiceCollection services)
    {
        services.AddSingleton<IArtisStreamProvider, ArtisStreamProvider>();
        services.AddSingleton(typeof(ILocalMessageBus<>), typeof(LocalMessageBus<>));
        services.AddHostedService<StreamHandlingBackgroundService>();
    }

    /// <summary>
    /// Gets the stream provider with the specified <paramref name="name"/>.
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="name">The provider name.</param>
    /// <returns>The stream provider.</returns>
    public static IArtisStreamProvider GetArtisStreamProvider(this IClusterClient client, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        try
        {
            return client.ServiceProvider.GetRequiredService<IArtisStreamProvider>();
        }
        catch (InvalidOperationException ex)
        {
            // We used to throw KeyNotFoundException before, keep it like this for backward compatibility
            throw new KeyNotFoundException($"Stream provider '{name}' not found", ex);
        }
    }
}