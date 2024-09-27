using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using Streams.Grains.LocalPubSub;

namespace Streams.Grains.ClusterPubSub;

public class ClusterPubSubSingletonGrain(ILogger<ClusterPubSubSingletonGrain> logger) : IClusterPubSubGrain
{
    private readonly ConcurrentDictionary<string, ILocalPubSubGrain> _subscribers = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, int> _subscriberErrorCount = new(StringComparer.OrdinalIgnoreCase);

    public Task AddSiloAsync(SiloAddress siloAddress, ILocalPubSubGrain pubSub)
    {
        if (_subscribers.TryAdd(siloAddress.ToString(), pubSub))
        {
            logger.LogInformation("Add LocalPubSubGrain {LocalPubSubGrainId} to silo {SiloAddress}", pubSub.GetGrainId(), siloAddress);
        }
        return Task.CompletedTask;
    }

    public Task RemoveSiloAsync(SiloAddress siloAddress)
    {
        logger.LogInformation("Remove silo {SiloAddress} from subscribers", siloAddress);
        if (!_subscribers.Remove(siloAddress.ToString(), out _))
        {
            logger.LogError($"Failed to remove silo {siloAddress.ToString()}");
        }

        return Task.CompletedTask;
    }

    public async Task PublishAsync(string streamId, string message)
    {
        if (!_subscribers.Any())
        {
            return;
        }

        var subscribersToRemove = new List<string>();

        foreach (var subscriber in _subscribers)
        {
            // error handling - what should happen if a silo is not reachable anymore?
            try
            {
                await subscriber.Value.PublishAsync(streamId, message);
            }
            catch (Exception e)
            {
                var entry = _subscriberErrorCount.GetOrAdd(subscriber.Key, 0);
                entry += 1;

                if (entry >= 3)
                {
                    subscribersToRemove.Add(subscriber.Key);
                }
            }
        }

        // cleanup not reachable subscribers
        foreach (var key in subscribersToRemove)
        {
            _subscribers.TryRemove(key, out _);
            _subscriberErrorCount.TryRemove(key, out _);
        }
    }
}