using Orleans.Runtime;

namespace Streams.Models.Interfaces;

public interface IArtisStreamSubscriptionHandle
{
    Guid SubscriptionId { get; }
    String StreamId { get; }
    SiloAddress SiloAddress { get; }
    Task UnsubscribeAsync();
}