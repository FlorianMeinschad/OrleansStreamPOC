using Orleans;

namespace Streams.Grains.LocalPubSub;

public interface ILocalPubSubGrain : IGrainObserver
{
    public Task PublishAsync(object message);
}