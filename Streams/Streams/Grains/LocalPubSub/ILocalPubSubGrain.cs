using Orleans;
using Streams.Models.Interfaces;

namespace Streams.Grains.LocalPubSub;

public interface ILocalPubSubGrain : IGrainObserver
{
    public Task PublishAsync(string streamId, string message);
    public Task<IList<IArtisStreamSubscriptionHandle>> GetAllSubscriptionsAsync(string streamId);
}