namespace Streams.Models.Interfaces;

internal interface ILocalMessageBus<T>
{
    Task OnNextAsync(string streamId, T message);
    Task<IArtisStreamSubscriptionHandle> SubscribeAsync(string streamId, Func<T, Task> onNextAsync);
    Task<IArtisStreamSubscriptionHandle> SubscribeAsync(string streamId, Func<T, Task> onNextAsync, Func<Exception, Task> onErrorAsync);
    Task<IArtisStreamSubscriptionHandle> SubscribeAsync(string streamId, Func<T, Task> onNextAsync, Func<Exception, Task> onErrorAsync,  Func<Task> onCompletedAsync);
    Task<IList<IArtisStreamSubscriptionHandle>> GetAllSubscriptionsAsync(string streamId);
}