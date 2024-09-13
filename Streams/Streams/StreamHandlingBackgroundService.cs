using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using Streams.Extensions;
using Streams.Grains.ClusterSiloStreamProvider;
using Streams.Grains.LocalSiloStreamProvider;

namespace Streams;

public class StreamHandlingBackgroundService : BackgroundService
{
    private readonly IGrainFactory _grainFactory;
    private readonly ILocalSiloDetails _localSiloDetails;
    private readonly LocalStreamProviderGrain _locationBroadcaster;
    private ILogger<StreamHandlingBackgroundService> _logger;

    public StreamHandlingBackgroundService(ILocalSiloDetails localSiloDetails, IGrainFactory grainFactory, ILogger<StreamHandlingBackgroundService> logger)
    {
        _grainFactory = grainFactory;
        _localSiloDetails = localSiloDetails;
        _locationBroadcaster = new LocalStreamProviderGrain();
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        IClusterStreamProviderGrain clusterStreamProvider = _grainFactory.GetSingletonGrain<IClusterStreamProviderGrain>();
        SiloAddress localSiloAddress = _localSiloDetails.SiloAddress;
        ILocalStreamProviderGrain selfReference =
            _grainFactory.CreateObjectReference<ILocalStreamProviderGrain>(_locationBroadcaster);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await clusterStreamProvider.AddStreamProviderAsync(localSiloAddress, selfReference);
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