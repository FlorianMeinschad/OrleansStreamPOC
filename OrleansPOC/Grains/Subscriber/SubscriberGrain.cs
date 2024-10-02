using Microsoft.Extensions.Logging;
using Orleans.Placement;
using Orleans.Streams;

namespace OrleansPOC.Grains.Subscriber;

[PreferLocalPlacement]
public class SubscriberGrain : Grain, ISubscriberGrain, IAsyncObserver<string>
{
    private StreamSubscriptionHandle<string>? StreamSubscriptionHandle { get; set; }
    private readonly IAsyncStream<string> _stream;
    private readonly IGrainRuntime _grainRuntime;
    private readonly ILogger<SubscriberGrain> _logger;

    public SubscriberGrain(IClusterClient clusterClient, IGrainRuntime grainRuntime, ILogger<SubscriberGrain> logger)
    {
        var streamProvider = clusterClient.GetStreamProvider(StreamProviderIds.STREAM);
        _stream = streamProvider.GetStream<string>(StreamChannelIds.TEST_STREAM_ID);
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
        if (StreamSubscriptionHandle is null)
        {
            _logger.LogInformation("Create subscription for subscriber {SubscriberId}", this.GetPrimaryKeyString());
            StreamSubscriptionHandle = await _stream.SubscribeAsync(this);
        }
        else
        {
            _logger.LogInformation("Resume subscription {StreamId}, {HandleId}", StreamSubscriptionHandle.StreamId, StreamSubscriptionHandle.HandleId);
            StreamSubscriptionHandle = await StreamSubscriptionHandle.ResumeAsync(this);
        }

        _logger.LogInformation("Subscriber started successfully on silo {Silo}", _grainRuntime.SiloAddress);
        await base.OnActivateAsync(cancellationToken);
    }

    public override async Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        if (StreamSubscriptionHandle != null) {
            await StreamSubscriptionHandle.UnsubscribeAsync();
            StreamSubscriptionHandle = null;
        }

        _logger.LogInformation("Subscriber stopped successfully on silo {Silo}, because of {@Reason}", _grainRuntime.SiloAddress, reason);
        await base.OnDeactivateAsync(reason, cancellationToken);
    }

    public Task OnNextAsync(string item, StreamSequenceToken? token = null)
    {
        _logger.LogInformation("Subscriber grain {GrainId} on silo {Silo} received publication: {UniqueMessageId} - {Publication}", this.GetPrimaryKeyString(), _grainRuntime.SiloAddress, Guid.NewGuid().ToString().Substring(0, 8), item);
        return Task.CompletedTask;
    }

    public Task OnCompletedAsync()
    {
        _logger.LogInformation("Publisher stopped successfully");
        return Task.CompletedTask;
    }

    public Task OnErrorAsync(Exception ex)
    {
        _logger.LogDebug("Error during subscribing to publisher Grain");
        return Task.CompletedTask;
    }
}