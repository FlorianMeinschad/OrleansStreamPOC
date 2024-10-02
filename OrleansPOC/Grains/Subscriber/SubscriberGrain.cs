using Microsoft.Extensions.Logging;
using Orleans.Placement;
using Orleans.Streams;
using Streams.Extensions;
using Streams.Models.Interfaces;

namespace OrleansPOC.Grains.Subscriber;

[PreferLocalPlacement]
public class SubscriberGrain : Grain, ISubscriberGrain
{
    private StreamSubscriptionHandle<string>? StreamSubscriptionHandle { get; set; }
    private readonly IArtisAsyncStream<string> _stream;
    private readonly IGrainRuntime _grainRuntime;
    private readonly ILogger<SubscriberGrain> _logger;
    private IArtisStreamSubscriptionHandle? _subscription;

    public SubscriberGrain(IClusterClient clusterClient, IGrainRuntime grainRuntime, ILogger<SubscriberGrain> logger)
    {
        var streamProvider = clusterClient.GetArtisStreamProvider(StreamProviderIds.STREAM);
        _stream = streamProvider.GetStream<string>(StreamChannelIds.TEST_STREAM_ID);
        _grainRuntime = grainRuntime;
        _logger = logger;
    }

    public Task StartAsync()
    {
        return Task.CompletedTask;
    }

    public async Task StopAsync()
    {
        if (_subscription != null)
        {
            _logger.LogInformation("Stop subscription {SubscriptionId} from {StreamId} on subscriber {GrainId}", _subscription.SubscriptionId, _subscription.StreamId, this.GetPrimaryKey());
            await _subscription.UnsubscribeAsync();
        }

        DeactivateOnIdle();
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _subscription = await _stream.SubscribeAsync(OnReceiveMessageAsync);
        _logger.LogInformation("Subscriber {GrainId} started successfully on silo {Silo}", this.GetPrimaryKey(), _grainRuntime.SiloAddress);
        await base.OnActivateAsync(cancellationToken);
    }

    private Task OnReceiveMessageAsync(string message)
    {
        _logger.LogInformation("Subscriber {GrainId} received message on silo: {SiloAddress}: {Message}", this.GetPrimaryKey(), _grainRuntime.SiloAddress, message);
        return Task.CompletedTask;
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
}