using Streams.Streaming.Interfaces;

namespace Streams.Grains.LocalPubSub;

public class LocalPubSubGrain(ILocalMessageBus messageBus) : ILocalPubSubGrain
{
    public Task PublishAsync(string streamId, string message)
    {
        return messageBus.PublishAsync(streamId, message);
    }
}