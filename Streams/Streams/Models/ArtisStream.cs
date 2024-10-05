using Streams.Grains.ClusterPubSub;
using Streams.Models.Interfaces;

namespace Streams.Models;

internal class ArtisAsyncStream<T>(
    string streamId,
    ILocalMessageBus<T> messageBus,
    IClusterPubSubGrain grain) : IArtisAsyncStream<T>
{
    public Task OnNextAsync(T message)
    {
        return grain.OnNextAsync(streamId, message);
    }

    public async Task<IArtisStreamSubscriptionHandle> SubscribeAsync(Func<T, Task> onNextAsync)
    {
        return await messageBus.SubscribeAsync(streamId, onNextAsync);
    }

    public async Task<IArtisStreamSubscriptionHandle> SubscribeAsync(Func<T, Task> onNextAsync, Func<Exception, Task> onErrorAsync)
    {
        return await messageBus.SubscribeAsync(streamId, onNextAsync, onErrorAsync);
    }

    public async Task<IArtisStreamSubscriptionHandle> SubscribeAsync(Func<T, Task> onNextAsync, Func<Exception, Task> onErrorAsync, Func<Task> onCompletedAsync)
    {
        return await messageBus.SubscribeAsync(streamId, onNextAsync, onErrorAsync, onCompletedAsync);
    }

    public async Task<IList<IArtisStreamSubscriptionHandle>> GetAllSubscriptionsAsync()
    {
        return await grain.GetAllSubscriptionsAsync<T>(streamId);
    }
}