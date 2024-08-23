using Microsoft.Extensions.Logging;
using Orleans.Streams;

namespace OrleansPOC.Grains.Publisher;

public class PublisherGrain : Grain, IPublisherGrain
{
    private IAsyncStream<string> _stream = null!;
    private IStreamProvider _streamProvider;
    private ILogger<PublisherGrain> _logger;
    private IGrainRuntime _grainRuntime;

    public PublisherGrain(ILogger<PublisherGrain> logger, IClusterClient client, IGrainRuntime grainRuntime)
    {
        _streamProvider = client.GetStreamProvider(ArtisStreamProviderIds.STREAM);
        _logger = logger;
        _grainRuntime = grainRuntime;
    }

    public Task StartAsync()
    {
        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        DeactivateOnIdle();
        return Task.CompletedTask;
    }

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _stream = _streamProvider.GetStream<string>(StreamChannelIds.STREAM_ID);

        _logger.LogInformation("Publisher started successfully on Sile {Silo}", _grainRuntime.SiloAddress);

        this.RegisterGrainTimer<object>(_ =>
        {
            return _stream.OnNextAsync("Message sent from publisher");
        }, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));

        return base.OnActivateAsync(cancellationToken);
    }
}