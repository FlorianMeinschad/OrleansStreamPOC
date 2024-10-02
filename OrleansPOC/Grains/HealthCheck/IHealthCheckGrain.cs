namespace OrleansPOC.Grains.HealthCheck;

public interface IHealthCheckGrain : IGrainWithGuidKey
{
    public Task CheckAsync();
}