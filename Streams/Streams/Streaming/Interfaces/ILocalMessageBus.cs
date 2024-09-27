namespace Streams.Streaming.Interfaces;

public interface ILocalMessageBus
{
    Task PublishAsync(string streamId, string message);
    IDisposable Subscribe(string streamIdc, Func<string, Task> handler);
}