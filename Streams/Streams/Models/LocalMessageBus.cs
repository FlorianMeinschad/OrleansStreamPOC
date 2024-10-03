using System.Collections.Concurrent;
using Streams.Models.Interfaces;

namespace Streams.Models;

internal record Subscriber<T>(Func<T, Task> Callback)
{
    public Guid SubscriptionId { get; } = Guid.NewGuid();
}

internal class LocalMessageBus<T> : ILocalMessageBus<T>
{
    // holds lists of local silo subscribers
    private readonly ConcurrentDictionary<string, List<Subscriber<T>>> _subs = new();

    public Task PublishAsync(string streamId, T message)
    {
        if (_subs.TryGetValue(streamId, out var subscribers))
        {
            var tasks = new Task[subscribers.Count];
            for (int i = 0; i < subscribers.Count; i++)
            {
                try
                {
                    if (message != null)
                    {
                        tasks[i] = subscribers[i].Callback(message);
                    }
                }
                catch (Exception e)
                {
                    // ignore
                }
            }

            return Task.WhenAll(tasks);
        }

        return Task.CompletedTask;
    }

    public Task<IArtisStreamSubscriptionHandle> SubscribeAsync(string streamId, Func<T, Task> callback)
    {
        var sub = _subs.GetOrAdd(streamId, new List<Subscriber<T>>());
        var subscription = new Subscriber<T>(callback);
        sub.Add(subscription);

        var subscriptionHandle = new Disposable(DisposeAction);
        return Task.FromResult(ArtisStreamSubscriptionHandle.CreateAsync(streamId, subscriptionHandle));

        void DisposeAction()
        {
            if (_subs.TryGetValue(streamId, out var subscribers))
            {
                subscribers.Remove(subscription);
            }
        }
    }

    public Task<IList<IArtisStreamSubscriptionHandle>> GetAllSubscriptionsAsync(string streamId)
    {
        throw new NotImplementedException();

        /*
        return _subs
            .Where(s => s.Key.Equals(streamId))
            .SelectMany(s => ValueTask.FromResult<IAsyncDisposable>(new Disposable(s.Value)))
            .ToList();
        */
    }

    private class Disposable(Action action) : IAsyncDisposable
    {
        public async ValueTask DisposeAsync()
        {
            action();
            await Task.CompletedTask;
        }
    }
}