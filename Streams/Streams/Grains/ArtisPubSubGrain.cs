using System.Collections.Concurrent;
using Orleans;

namespace Streams.Grains;


public interface IArtisPubSubGrain : IGrainWithGuidKey
{
    Task RegisterSubscriber(string streamId, Guid subscriberId);
    Task UnregisterSubscriber(string streamId, Guid subscriberId);
    Task PublishEvent(string streamId, object evt);
}

// PubSubGrain in Orleans is responsible for managing subscribers for a stream.
// It holds information about which grains are subscribed to which streams and ensures
// that events are dispatched to the right subscribers.
public class ArtisPubSubGrain : Grain, IArtisPubSubGrain
{
    private readonly ConcurrentDictionary<string, HashSet<Guid>> _subscriptions = new();

    public Task RegisterSubscriber(string streamId, Guid subscriberId)
    {
        if (!_subscriptions.ContainsKey(streamId))
        {
            _subscriptions[streamId] = new HashSet<Guid>();
        }

        _subscriptions[streamId].Add(subscriberId);
        Console.WriteLine($"Subscriber {subscriberId} subscribed to stream {streamId}.");
        return Task.CompletedTask;
    }

    public Task UnregisterSubscriber(string streamId, Guid subscriberId)
    {
        if (_subscriptions.ContainsKey(streamId))
        {
            _subscriptions[streamId].Remove(subscriberId);
            if (_subscriptions[streamId].Count == 0)
            {
                _subscriptions.Remove(streamId, out var _);
            }
        }

        Console.WriteLine($"Subscriber {subscriberId} unsubscribed from stream {streamId}.");
        return Task.CompletedTask;
    }

    public async Task PublishEvent(string streamId, object evt)
    {
        if (!_subscriptions.ContainsKey(streamId))
        {
            Console.WriteLine($"No subscribers for stream {streamId}. Event dropped.");
            return;
        }

        /*
        var subscriberGrains = [];
         _subscriptions[streamId]
            .Select(subscriberId => GrainFactory.GetGrain<ISubscriberGrain>(subscriberId));

        foreach (var subscriber in subscriberGrains)
        {
            await subscriber.ReceiveNotification(evt);
        }
        */

        // Console.WriteLine($"Published event to {subscriberGrains.Count()} subscribers for stream {streamId}.");
    }
}