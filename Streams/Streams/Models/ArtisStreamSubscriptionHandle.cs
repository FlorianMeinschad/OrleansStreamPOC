using Orleans;
using Orleans.Runtime;
using Streams.Models.Interfaces;

namespace Streams.Models;

[GenerateSerializer]
internal sealed record ArtisStreamSubscriptionHandle(Guid SubscriptionId, string StreamId, SiloAddress SiloAddress, IUnsubscribe UnsubscribeHandler) : IArtisStreamSubscriptionHandle
{
    public static IArtisStreamSubscriptionHandle Create(Guid subscriptionId, string streamId, SiloAddress siloAddress, IUnsubscribe unsubscribeMethod)
    {
        return new ArtisStreamSubscriptionHandle(subscriptionId, streamId, siloAddress, unsubscribeMethod);
    }

    public async Task UnsubscribeAsync()
    {
        await UnsubscribeHandler.UnsubscribeAsync();
    }
}