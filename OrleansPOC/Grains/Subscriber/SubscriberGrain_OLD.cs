using Microsoft.Extensions.Logging;
using Orleans.Placement;
using Orleans.Streams;
using Serilog;

namespace OrleansPOC.Grains.Subscriber;
/*
[PreferLocalPlacement]
public class SubscriberGrainOLD : Grain, ISubscriberGrain, IAsyncObserver<string>
{
    private StreamSubscriptionHandle<string>? StreamSubscriptionHandle { get; set; }
    private readonly IAsyncStream<string> _stream;
    private readonly IGrainRuntime _grainRuntime;
    private readonly ILogger<SubscriberGrain> _logger;

    public SubscriberGrainOLD(IClusterClient clusterClient, IGrainRuntime grainRuntime, ILogger<SubscriberGrain> logger)
    {
        var streamProvider = clusterClient.GetStreamProvider(StreamProviderIds.STREAM);
        _stream = streamProvider.GetStream<string>(StreamChannelIds.STREAM_ID);
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
        try
        {
            await ResumeOrCreateSubscriptionAsync();
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Error on resuming stream");
            await CleanupStaleSubscriptions();
            await CreateNewSubscription();
        }

        _logger.LogInformation("Subscriber started successfully on Sile {Silo}", _grainRuntime.SiloAddress);
        await base.OnActivateAsync(cancellationToken);
    }

    private async Task ResumeOrCreateSubscriptionAsync()
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
    }

    private async Task CleanupStaleSubscriptions()
    {
        var subscriptions = await _stream.GetAllSubscriptionHandles();
        if (subscriptions.Count > 1)
        {
            _logger.LogCritical("More than one subscription handle is currently subscribed.");
        }

        foreach (var subscription in subscriptions)
        {
            _logger.LogWarning("Unsubscribe from subscription {StreamId}, {HandleId}", subscription.StreamId, subscription.HandleId);
            await subscription.UnsubscribeAsync();
        }
    }

    private async Task CreateNewSubscription()
    {
        StreamSubscriptionHandle = await _stream.SubscribeAsync(this);
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

    public Task OnNextAsync(string item, StreamSequenceToken? token = null)
    {
        _logger.LogInformation("Subscriber Grain {GrainId} on Silo {Silo} received publication: {Publication}", this.GetPrimaryKeyString(), _grainRuntime.SiloAddress, item);
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
*/