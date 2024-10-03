using Streams.Grains.ClusterPubSub;
using Streams.Models.Interfaces;

namespace Streams.Models;

internal class ArtisAsyncStream<T>(
    string streamId,
    ILocalMessageBus<T> messageBus,
    IClusterPubSubGrain grain) : IArtisAsyncStream<T>
{
    public Task PublishAsync(T message)
    {
        return grain.PublishAsync(streamId, message);
    }

    public async Task<IArtisStreamSubscriptionHandle> SubscribeAsync(Func<T, Task> callback)
    {
        return await messageBus.SubscribeAsync(streamId, callback);
    }

    public async Task<IList<IArtisStreamSubscriptionHandle>> GetAllSubscriptionsAsync()
    {
        return await grain.GetAllSubscriptionsAsync<T>(streamId);
    }
}