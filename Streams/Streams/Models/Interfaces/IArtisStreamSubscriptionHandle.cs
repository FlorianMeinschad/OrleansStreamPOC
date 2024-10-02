namespace Streams.Models.Interfaces;

public interface IArtisStreamSubscriptionHandle
{
    Guid SubscriptionId { get; }
    String StreamId { get; }
    Task UnsubscribeAsync();
}