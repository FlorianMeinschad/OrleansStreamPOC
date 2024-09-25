using Orleans.Runtime;
using Streams.Grains.ClusterPubSub;
using Streams.Streaming.Interfaces;

namespace Streams.Streaming;

public class ArtisAsyncStream<T>(string streamId, ILocalMessageBus messageBus, IClusterPubSubGrain grain, SiloAddress siloAddress) : IArtisAsyncStream<T>
{
    public Task PublishAsync(object message)
    {
        return grain.PublishAsync(message);
    }

    public async Task<IAsyncDisposable> SubscribeAsync(Func<object, Task> onMessage)
    {
        var disposable = messageBus.Subscribe(streamId, onMessage);

        // This needs to keep track of the number of subscribers and appropriate add and remove silos
        // await grain.AddSiloAsync(siloAddress, this);

        return new Disposable(async () =>
        {
            disposable.Dispose();

            // Remove the grain when there are no more subscribers
            await grain.RemoveSiloAsync(siloAddress);
        });
    }

    class Disposable(Func<ValueTask> disposeAsync) : IAsyncDisposable
    {
        public ValueTask DisposeAsync()
        {
            return disposeAsync();
        }
    }
}