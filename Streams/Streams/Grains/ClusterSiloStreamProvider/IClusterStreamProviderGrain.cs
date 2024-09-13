using Orleans;
using Orleans.Runtime;
using Streams.Extensions;
using Streams.Grains.LocalSiloStreamProvider;

namespace Streams.Grains.ClusterSiloStreamProvider;

public interface IClusterStreamProviderGrain : IGrainWithSingletonKey
{
    public Task AddStreamProviderAsync(SiloAddress host, ILocalStreamProviderGrain streamProvider);
}