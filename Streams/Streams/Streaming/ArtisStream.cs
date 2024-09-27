using Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using Orleans.Streams.Core;
using Streams.Grains.ClusterPubSub;
using Streams.Streaming.Interfaces;

namespace Streams.Streaming;

public class ArtisAsyncStream<T>(
    string streamId,
    ILocalMessageBus messageBus,
    IClusterPubSubGrain grain,
    SiloAddress siloAddress) : IArtisAsyncStream<T>
{
    public Task PublishAsync(string message)
    {
        return grain.PublishAsync(streamId, message);
    }

    public Task SubscribeAsync(Func<string, Task> onMessage)
    {
        var disposable = messageBus.Subscribe(streamId, onMessage);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<StreamSubscription>> GetAllSubscriptionHandles()
    {
        throw new NotImplementedException();
    }
}