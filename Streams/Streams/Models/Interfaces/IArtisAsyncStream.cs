namespace Streams.Models.Interfaces;

public interface IArtisAsyncStream<T>
{
    Task OnNextAsync(T message);
    Task<IArtisStreamSubscriptionHandle> SubscribeAsync(Func<T, Task> onNextAsync);
    Task<IArtisStreamSubscriptionHandle> SubscribeAsync(Func<T, Task> onNextAsync, Func<Exception, Task> onErrorAsync);
    Task<IArtisStreamSubscriptionHandle> SubscribeAsync(Func<T, Task> onNextAsync, Func<Exception, Task> onErrorAsync, Func<Task> onCompletedAsync);
    Task<IList<IArtisStreamSubscriptionHandle>> GetAllSubscriptionsAsync();
}