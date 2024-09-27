using System.Collections.Concurrent;
using Streams.Streaming.Interfaces;

namespace Streams.Streaming;

public class LocalMessageBus : ILocalMessageBus
{
    private readonly ConcurrentDictionary<string, List<Func<string, Task>>> _subs = new();

    public Task PublishAsync(string streamId, string message)
    {
        if (_subs.TryGetValue(streamId, out var subscribers))
        {
            var tasks = new Task[subscribers.Count];
            for (int i = 0; i < subscribers.Count; i++)
            {
                try
                {
                    tasks[i] = subscribers[i](message);
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

    public IDisposable Subscribe(string streamId, Func<string, Task> handler)
    {
        var sub = _subs.GetOrAdd(streamId, new List<Func<string, Task>>());
        lock (sub)
        {
            sub.Add(handler);
        }

        return new Disposable(() =>
        {
            lock (sub)
            {
                if (_subs.TryGetValue(streamId, out var subscribers))
                {
                    subscribers.Remove(handler);
                }
            }
        });
    }

    private class Disposable : IDisposable
    {
        private readonly Action _action;

        public Disposable(Action action)
        {
            _action = action;
        }

        public void Dispose()
        {
            _action();
        }
    }
}