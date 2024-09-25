namespace Streams.Streaming.Interfaces;

public interface ILocalMessageBus
{
    Task PublishAsync(string topic, byte[] message);
    IDisposable Subscribe(string topic, Func<byte[], Task> onMessage);
}