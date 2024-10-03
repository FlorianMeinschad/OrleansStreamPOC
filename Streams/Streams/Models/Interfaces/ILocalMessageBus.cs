namespace Streams.Models.Interfaces;

internal interface ILocalMessageBus<T>
{
    Task PublishAsync(string streamId, T message);
    Task<IArtisStreamSubscriptionHandle> SubscribeAsync(string streamId, Func<T, Task> callback);
    Task<IList<IArtisStreamSubscriptionHandle>> GetAllSubscriptionsAsync(string streamId);
}