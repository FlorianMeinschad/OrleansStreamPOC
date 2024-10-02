namespace Streams.Models.Interfaces;

public interface ILocalMessageBus
{
    Task PublishAsync(string streamId, string message);
    Task<IArtisStreamSubscriptionHandle> SubscribeAsync(string streamId, Func<string, Task> callback);
    Task<IList<IArtisStreamSubscriptionHandle>> GetAllSubscriptionsAsync(string streamId);
}