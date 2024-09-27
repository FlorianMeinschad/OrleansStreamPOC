using Orleans.Streams;

namespace Streams.Streaming.Interfaces;

public interface IArtisStreamProvider {
    public IArtisAsyncStream<T> GetStream<T>(string streamId);
}