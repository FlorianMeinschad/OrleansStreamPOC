using Microsoft.Extensions.Logging;
using Streams.Extensions;
using Streams.Models.Interfaces;

namespace OrleansPOC.Grains.Publisher;

public class PublisherGrain(ILogger<PublisherGrain> logger, IClusterClient client, IGrainRuntime grainRuntime)
    : Grain, IPublisherGrain
{
    private IArtisAsyncStream<string> _stream = null!;
    private readonly IArtisStreamProvider _streamProvider = client.GetArtisStreamProvider(StreamProviderIds.STREAM);

    public Task StartAsync(TimeSpan interval)
    {
        _stream = _streamProvider.GetStream<string>(StreamChannelIds.TEST_STREAM_ID);
        logger.LogInformation("Publisher started successfully on silo {Silo}", grainRuntime.SiloAddress);

        this.RegisterGrainTimer<object>(_ => _stream.OnNextAsync("Publisher says hello"), null, TimeSpan.FromSeconds(1), interval);
        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        DeactivateOnIdle();
        return Task.CompletedTask;
    }
}