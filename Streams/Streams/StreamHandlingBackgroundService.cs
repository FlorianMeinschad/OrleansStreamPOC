using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using Streams.Extensions;
using Streams.Grains.ClusterPubSub;
using Streams.Grains.LocalPubSub;

namespace Streams;

public class StreamHandlingBackgroundService : BackgroundService
{
    private readonly IGrainFactory _grainFactory;
    private readonly ILocalSiloDetails _localSiloDetails;
    private readonly LocalPubSubGrain _locationBroadcaster;
    private ILogger<StreamHandlingBackgroundService> _logger;

    public StreamHandlingBackgroundService(ILocalSiloDetails localSiloDetails, IGrainFactory grainFactory, ILogger<StreamHandlingBackgroundService> logger)
    {
        _grainFactory = grainFactory;
        _localSiloDetails = localSiloDetails;
        _locationBroadcaster = new LocalPubSubGrain();
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        IClusterPubSubGrain clusterPubSub = _grainFactory.GetSingletonGrain<IClusterPubSubGrain>();
        SiloAddress localSiloAddress = _localSiloDetails.SiloAddress;
        ILocalPubSubGrain selfReference =
            _grainFactory.CreateObjectReference<ILocalPubSubGrain>(_locationBroadcaster);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await clusterPubSub.AddSiloAsync(localSiloAddress, selfReference);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error polling stream provider hub list");
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