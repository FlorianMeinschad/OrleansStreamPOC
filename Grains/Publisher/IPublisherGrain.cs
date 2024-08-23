namespace OrleansPOC.Grains.Publisher;

public interface IPublisherGrain : IGrainWithGuidKey
{
    public Task StartAsync();
    public Task StopAsync();
}