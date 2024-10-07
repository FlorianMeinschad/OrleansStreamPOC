namespace OrleansPOC.Grains.Publisher;

public interface IPublisherGrain : IGrainWithGuidKey
{
    public Task StartAsync(TimeSpan interval);
    public Task StopAsync();
    public Task CompleteAsync();
}