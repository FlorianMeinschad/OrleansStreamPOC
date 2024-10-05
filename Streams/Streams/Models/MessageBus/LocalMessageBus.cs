using System.Collections.Concurrent;
using Orleans.Runtime;
using Streams.Models.Interfaces;

namespace Streams.Models.MessageBus;

internal class LocalMessageBus<T>(ILocalSiloDetails localSiloDetails) : ILocalMessageBus<T>
{
    // holds lists of local silo subscribers
    private readonly ConcurrentDictionary<string, ConcurrentBag<Subscriber<T>>> _subs = new();

    public Task OnNextAsync(string streamId, T message)
    {
        if (!_subs.TryGetValue(streamId, out var subscribers)) {
            return Task.CompletedTask;
        }

        var tasks = subscribers.Select(async s =>
            {
                try
                {
                    return s.OnNextAsync(message);
                }
                catch (Exception ex)
                {
                    if (s.OnErrorAsync != null)
                    {
                        await s.OnErrorAsync(ex);
                    }

                    return Task.CompletedTask;
                }
            }).ToArray();

        return Task.WhenAll(tasks);
    }

    public Task<IArtisStreamSubscriptionHandle> SubscribeAsync(string streamId, Func<T, Task> onNextAsync)
    {
        var subscriber = new Subscriber<T>(onNextAsync);
        return SubscribeInternalAsync(streamId, subscriber);
    }

    public Task<IArtisStreamSubscriptionHandle> SubscribeAsync(string streamId, Func<T, Task> onNextAsync, Func<Exception, Task> onErrorAsync)
    {
        var subscriber = new Subscriber<T>(onNextAsync, onErrorAsync);
        return SubscribeInternalAsync(streamId, subscriber);
    }

    public Task<IArtisStreamSubscriptionHandle> SubscribeAsync(string streamId, Func<T, Task> onNextAsync, Func<Exception, Task> onErrorAsync, Func<Task> onCompletedAsync)
    {
        var subscriber = new Subscriber<T>(onNextAsync, onErrorAsync, onCompletedAsync);
        return SubscribeInternalAsync(streamId, subscriber);
    }

    public Task<IList<IArtisStreamSubscriptionHandle>> GetAllSubscriptionsAsync(string streamId)
    {
        if (!_subs.TryGetValue(streamId, out var subscribers))
        {
            // If no subscriptions exist for the streamId, return an empty list
            return Task.FromResult<IList<IArtisStreamSubscriptionHandle>>(new List<IArtisStreamSubscriptionHandle>());
        }

        var handles = subscribers.Select(subscriber =>
            ArtisStreamSubscriptionHandle.Create(subscriber.Id, streamId, localSiloDetails.SiloAddress, new UnsubscribeHandler (async () =>
            {
                await RemoveSubscriberAsync(streamId, subscriber);
            }))
        ).ToList();

        return Task.FromResult<IList<IArtisStreamSubscriptionHandle>>(handles);
    }

    private Task<IArtisStreamSubscriptionHandle> SubscribeInternalAsync(string streamId, Subscriber<T> subscriber)
    {
        var streamSubscription = _subs.GetOrAdd(streamId, new ConcurrentBag<Subscriber<T>>());
        streamSubscription.Add(subscriber);

        return Task.FromResult(ArtisStreamSubscriptionHandle.Create(subscriber.Id, streamId, localSiloDetails.SiloAddress, new UnsubscribeHandler(async () =>
        {
            await RemoveSubscriberAsync(streamId, subscriber);
        })));
    }

    private Task RemoveSubscriberAsync(string streamId, Subscriber<T> subscriber)
    {
        if (_subs.TryGetValue(streamId, out var subscribers))
        {
            var updatedSubscribers = new ConcurrentBag<Subscriber<T>>(subscribers.Where(s => s.Id != subscriber.Id));
            _subs[streamId] = updatedSubscribers;
        }

        return Task.CompletedTask;
    }
}