using Microsoft.Extensions.DependencyInjection;
using Streams.Streaming.Interfaces;

namespace Streams.Streaming;

public static class StreamingExtensions
{
    public static void AddArtisStreaming(IServiceCollection services)
    {
        services.AddSingleton<IArtisStreamProvider, ArtisStreamProvider>();
    }
}