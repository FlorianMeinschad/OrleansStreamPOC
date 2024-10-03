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

    public Task<IList<IArtisStreamSubscriptionHandle>> GetAllSubscriptionsAsync()
    {
        throw new NotImplementedException();
        // return grain.GetAllSubscriptionsAsync();
    }
}