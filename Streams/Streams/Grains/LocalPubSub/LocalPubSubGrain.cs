using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using Streams.Streaming.Interfaces;

namespace Streams.Grains.LocalPubSub;

public class LocalPubSubGrain(ILogger<BackgroundService> logger, ILocalMessageBus messageBus, ILocalSiloDetails localSiloDetails) : Grain, ILocalPubSubGrain
{
    public Task PublishAsync(string streamId, string message)
    {
        return messageBus.PublishAsync(streamId, message);
    }

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("ClusterPubSubSingletonGrain activated on silo {Silo}", localSiloDetails.SiloAddress);
        return Task.CompletedTask;
    }

    public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        logger.LogWarning("ClusterPubSubSingletonGrain deactivated on silo {Silo}", localSiloDetails.SiloAddress);
        return Task.CompletedTask;
    }
}