namespace OrleansPOC.Grains.Subscriber;

public interface ISubscriberGrain : IGrainWithGuidKey
{
    public Task StartAsync();
    public Task StopAsync();
}