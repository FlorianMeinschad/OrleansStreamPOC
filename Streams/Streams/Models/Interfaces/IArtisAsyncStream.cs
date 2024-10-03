namespace Streams.Models.Interfaces;

public interface IArtisAsyncStream<T>
{
    Task PublishAsync(T message);
    Task<IArtisStreamSubscriptionHandle> SubscribeAsync(Func<T, Task> onMessage);
    Task<IList<IArtisStreamSubscriptionHandle>> GetAllSubscriptionsAsync();
}