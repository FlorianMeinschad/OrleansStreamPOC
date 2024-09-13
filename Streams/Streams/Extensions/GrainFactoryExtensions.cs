using Orleans;

namespace Streams.Extensions;

public static class GrainFactoryExtensions
{
    public static T GetSingletonGrain<T>(this IGrainFactory grainFactory) where T : IGrainWithSingletonKey {
        return grainFactory.GetGrain<T>(Guid.Empty);
    }
}