using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using Streams.Models.Interfaces;

namespace Streams.Grains.LocalPubSub;

internal class LocalPubSubGrain(ILogger<BackgroundService> logger, IServiceProvider serviceProvider, ILocalSiloDetails localSiloDetails) : Grain, ILocalPubSubGrain
{
    public Task PublishAsync<T>(string streamId, T message)
    {
        var messageBus = serviceProvider.GetRequiredService<ILocalMessageBus<T>>();
        return messageBus.PublishAsync(streamId, message);
    }

    public Task<IList<IArtisStreamSubscriptionHandle>> GetAllSubscriptionsAsync<T>(string streamId)
    {
        var messageBus = serviceProvider.GetRequiredService<ILocalMessageBus<T>>();
        return messageBus.GetAllSubscriptionsAsync(streamId);
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