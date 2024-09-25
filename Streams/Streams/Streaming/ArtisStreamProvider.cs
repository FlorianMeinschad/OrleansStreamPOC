using System.Collections.Concurrent;
using Orleans;
using Orleans.Runtime;
using Streams.Exceptions;
using Streams.Extensions;
using Streams.Grains.ClusterPubSub;
using Streams.Streaming.Interfaces;

namespace Streams.Streaming;

public class ArtisStreamProvider(IGrainFactory grainFactory, ILocalMessageBus messageBus, ILocalSiloDetails localSiloDetails) : IArtisStreamProvider
{
    private readonly ConcurrentDictionary<string, object> _streams = new();

    public IArtisAsyncStream<T> GetStream<T>(string streamId)
    {
        var grain = grainFactory.GetSingletonGrain<IClusterPubSubGrain>();
        var stream = _streams.GetOrAdd(streamId, () => new ArtisAsyncStream<T>(streamId, messageBus, grain, localSiloDetails.SiloAddress));

        var streamOfT = stream as IArtisAsyncStream<T>;
        if (streamOfT is null)
        {
            throw new StreamException($"Stream type mismatch. A stream can only support a single type of data. The generic type of the stream requested ({typeof(T)}) does not match the previously requested type ({stream.GetType().GetGenericArguments().FirstOrDefault()}).");
        }

        return streamOfT;
    }

    public string Name { get; }
}