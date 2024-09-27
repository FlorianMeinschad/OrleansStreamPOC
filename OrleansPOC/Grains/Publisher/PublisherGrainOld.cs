using Microsoft.Extensions.Logging;
using Orleans.Streams;
using Orleans.Streams.Core;

namespace OrleansPOC.Grains.Publisher;

/*
public class PublisherGrainOld : Grain, IPublisherGrain
{
    private IAsyncStream<string> _stream = null!;
    private readonly IStreamProvider _streamProvider;
    private readonly ILogger<PublisherGrain> _logger;
    private readonly IGrainRuntime _grainRuntime;

    public PublisherGrainOld(ILogger<PublisherGrain> logger, IClusterClient client, IGrainRuntime grainRuntime)
    {
        _streamProvider = client.GetStreamProvider(StreamProviderIds.STREAM);
        _logger = logger;
        _grainRuntime = grainRuntime;
    }

    public Task StartAsync(TimeSpan interval)
    {
        _stream = _streamProvider.GetStream<string>(StreamChannelIds.STREAM_ID);
        _logger.LogInformation("Publisher started successfully on Silo {Silo}", _grainRuntime.SiloAddress);

        this.RegisterGrainTimer<object>(_ => _stream.OnNextAsync("Message sent from publisher"), null, TimeSpan.FromSeconds(1), interval);
        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        DeactivateOnIdle();
        return Task.CompletedTask;
    }
}
*/