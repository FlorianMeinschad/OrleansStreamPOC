using Orleans;
using Streams.Models.Interfaces;

namespace Streams.Grains.LocalPubSub;

public interface ILocalPubSubGrain : IGrainObserver
{
    public Task OnNextAsync<T>(string streamId, T message);
    public Task<IList<IArtisStreamSubscriptionHandle>> GetAllSubscriptionsAsync<T>(string streamId);
}