using Orleans.Streams;
using Orleans.Streams.Core;

namespace Streams.Streaming.Interfaces;

public interface IArtisAsyncStream<T>
{
    Task PublishAsync(string message);
    Task SubscribeAsync(Func<string, Task> onMessage);
    Task<IEnumerable<StreamSubscription>> GetAllSubscriptionHandles();
}