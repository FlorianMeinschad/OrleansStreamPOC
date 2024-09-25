using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using Streams.Grains.LocalPubSub;

namespace Streams.Grains.ClusterPubSub;

public class ClusterPubSubSingletonGrain(ILogger logger, ConcurrentDictionary<string, ILocalPubSubGrain> subscribers) : IClusterPubSubGrain
{
    public Task AddSiloAsync(SiloAddress siloAddress, ILocalPubSubGrain pubSub)
    {
        if (subscribers.TryAdd(siloAddress.ToString(), pubSub))
        {
            logger.LogInformation("Add Silo to ");
        }
        return Task.CompletedTask;
    }

    public Task RemoveSiloAsync(SiloAddress siloAddress)
    {
        if (!subscribers.TryRemove(siloAddress.ToString(), out _))
        {
            logger.LogError($"Failed to remove silo {siloAddress.ToString()}");
        }

        return Task.CompletedTask;
    }

    public Task PublishAsync(object message)
    {
        if (!subscribers.Any())
        {
            return Task.CompletedTask;
        }

        var tasks = new List<Task>();
        foreach (var subscriber in subscribers.Values)
        {
            tasks.Add(subscriber.PublishAsync(message));
        }
        return Task.WhenAll(tasks);
    }
}