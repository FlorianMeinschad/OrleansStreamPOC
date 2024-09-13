using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using Streams.Grains.LocalSiloStreamProvider;

namespace Streams.Grains.ClusterSiloStreamProvider;

public class ClusterStreamProviderGrain : IClusterStreamProviderGrain
{
    private IClusterMembershipService ClusterMembershipService { get; }
    private Dictionary<SiloAddress, ILocalStreamProviderGrain> StreamProviders { get; } = new Dictionary<SiloAddress, ILocalStreamProviderGrain>();

    private ILogger Logger { get; }

    public ClusterStreamProviderGrain(IClusterMembershipService clusterMembershipServiceService, ILogger logger)
    {
        ClusterMembershipService = clusterMembershipServiceService;
        // Logger = grainLoggerProvider.GetGrainLogger(this);
        Logger = logger;
    }

    public Task AddStreamProviderAsync(SiloAddress host, ILocalStreamProviderGrain streamProvider)
    {
        StreamProviders[host] = streamProvider;
        throw new NotImplementedException();
    }
}