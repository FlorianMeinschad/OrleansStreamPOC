using Orleans.Runtime;
using Streams.Extensions;
using Streams.Grains.LocalPubSub;

namespace Streams.Grains.ClusterPubSub;

public interface IClusterPubSubGrain : IGrainWithSingletonKey
{
    Task AddSiloAsync(SiloAddress siloAddress, ILocalPubSubGrain pubSub);
    Task RemoveSiloAsync(SiloAddress siloAddress);
    Task PublishAsync(string streamId, string message);
}