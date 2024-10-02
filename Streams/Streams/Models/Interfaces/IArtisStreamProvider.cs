namespace Streams.Models.Interfaces;

public interface IArtisStreamProvider {
    public IArtisAsyncStream<T> GetStream<T>(string streamId);
}