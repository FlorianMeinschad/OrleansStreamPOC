namespace Streams.Streaming.Interfaces;

public interface IArtisAsyncStream<T>
{
    Task PublishAsync(object message);
    Task<IAsyncDisposable> SubscribeAsync(Func<object, Task> onMessage);
}