namespace Streams.Models.Interfaces;

public interface IArtisAsyncStream<T>
{
    Task PublishAsync(string message);
    Task<IArtisStreamSubscriptionHandle> SubscribeAsync(Func<string, Task> onMessage);
    Task<IList<IArtisStreamSubscriptionHandle>> GetAllSubscriptionsAsync();
}