using Streams.Models.Interfaces;

namespace Streams.Models;

internal record ArtisStreamSubscriptionHandle(string StreamId, IAsyncDisposable Subscription) : IArtisStreamSubscriptionHandle
{
    public Guid SubscriptionId { get; } = Guid.NewGuid();

    public static IArtisStreamSubscriptionHandle CreateAsync(string streamId, IAsyncDisposable subscription)
    {
        return new ArtisStreamSubscriptionHandle(streamId, subscription);
    }

    public async Task UnsubscribeAsync()
    {
        await Subscription.DisposeAsync();
    }
}