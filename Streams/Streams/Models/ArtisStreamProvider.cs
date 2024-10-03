using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Orleans;
using Streams.Exceptions;
using Streams.Extensions;
using Streams.Grains.ClusterPubSub;
using Streams.Models.Interfaces;

namespace Streams.Models;

/// <summary>
/// Erstellt oder liefert Stream.
/// </summary>
internal class ArtisStreamProvider(IGrainFactory grainFactory, IServiceProvider serviceProvider) : IArtisStreamProvider
{
    private readonly ConcurrentDictionary<string, object> _streams = new();

    public IArtisAsyncStream<T> GetStream<T>(string streamId)
    {
        var messageBus = serviceProvider.GetRequiredService<ILocalMessageBus<T>>();
        var grain = grainFactory.GetSingletonGrain<IClusterPubSubGrain>();
        var stream = _streams.GetOrAdd(streamId, new ArtisAsyncStream<T>(streamId, messageBus, grain));

        var streamOfT = stream as IArtisAsyncStream<T>;
        if (streamOfT is null)
        {
            throw new StreamException($"Stream type mismatch. A stream can only support a single type of data. The generic type of the stream requested does not match the previously requested type ({stream.GetType().GetGenericArguments().FirstOrDefault()}).");
        }

        return streamOfT;
    }
}