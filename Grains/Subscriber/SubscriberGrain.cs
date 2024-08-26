using Microsoft.Extensions.Logging;
using Orleans.Streams;

namespace OrleansPOC.Grains.Subscriber;

public class SubscriberGrain : Grain, ISubscriberGrain
{
    private StreamSubscriptionHandle<string>? StreamSubscriptionHandle { get; set; }
    private readonly IStreamProvider _streamProvider;
    private readonly IGrainRuntime _grainRuntime;
    private readonly ILogger<SubscriberGrain> _logger;

    public SubscriberGrain(IClusterClient clusterClient, IGrainRuntime grainRuntime, ILogger<SubscriberGrain> logger)
    {
        _streamProvider = clusterClient.GetStreamProvider(StreamProviderIds.STREAM);
        _grainRuntime = grainRuntime;
        _logger = logger;
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

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        StreamSubscriptionHandle = await _streamProvider
            .GetStream<string>(StreamChannelIds.STREAM_ID)
            .SubscribeAsync((publication, _) => OnUpdateSuccessAsync(publication), OnUpdateErrorAsync);

        _logger.LogInformation("Subscriber started successfully on Sile {Silo}", _grainRuntime.SiloAddress);
        await base.OnActivateAsync(cancellationToken);
    }

    public override async Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        if (StreamSubscriptionHandle != null) {
            await StreamSubscriptionHandle.UnsubscribeAsync();
            StreamSubscriptionHandle = null;
        }

        _logger.LogInformation("Subscriber stopped successfully");
        await base.OnDeactivateAsync(reason, cancellationToken);
    }

    private Task OnUpdateSuccessAsync(string publication) {
        _logger.LogInformation("Subscriber Grain {GrainId} on Silo {Silo} received publication: {Publication}", this.GetPrimaryKeyString(), _grainRuntime.SiloAddress, publication);
        return Task.CompletedTask;
    }

    private Task OnUpdateErrorAsync(Exception arg) {
        _logger.LogDebug("Error during subscribing to publisher Grain");
        return Task.CompletedTask;
    }


}