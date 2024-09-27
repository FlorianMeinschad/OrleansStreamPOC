using System.Collections.Concurrent;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Concurrency;
using Orleans.Runtime;
using Streams.Extensions;
using Streams.Grains.ClusterPubSub;
using Streams.Grains.LocalPubSub;
using Streams.Streaming.Interfaces;

namespace Streams;

[Reentrant]
public class StreamHandlingBackgroundService(
    ILocalSiloDetails localSiloDetails,
    IGrainFactory grainFactory,
    ILocalMessageBus localMessageBus,
    ILogger<StreamHandlingBackgroundService> logger)
    : BackgroundService
{
    private readonly LocalPubSubGrain _locationBroadcaster = new(localMessageBus);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var clusterPubSub = grainFactory.GetSingletonGrain<IClusterPubSubGrain>();
        var localSiloAddress = localSiloDetails.SiloAddress;
        var selfReference = grainFactory.CreateObjectReference<ILocalPubSubGrain>(_locationBroadcaster);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await clusterPubSub.AddSiloAsync(localSiloAddress, selfReference);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error polling stream provider hub list");
            }

            if (!stoppingToken.IsCancellationRequested) {
                try {
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                } catch {
                    // Ignore cancellation exceptions, since cancellation is handled by the outer loop.
                }
            }
        }
    }
}