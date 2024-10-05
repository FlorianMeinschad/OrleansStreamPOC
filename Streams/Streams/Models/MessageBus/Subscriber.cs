namespace Streams.Models.MessageBus;

internal class Subscriber<T>
{
    public Guid Id { get; } = Guid.NewGuid();
    public Func<T, Task> OnNextAsync { init; get; }
    public Func<Exception, Task>? OnErrorAsync { init; get; }
    public Func<Task>? OnCompletedAsync { init; get; }

    public Subscriber(Func<T, Task> onNextAsync)
    {
        OnNextAsync = onNextAsync;
    }

    public Subscriber(Func<T, Task> onNextAsync, Func<Exception, Task> onErrorAsync)
    {
        OnNextAsync = onNextAsync;
        OnErrorAsync = onErrorAsync;
    }

    public Subscriber(Func<T, Task> onNextAsync, Func<Exception, Task> onErrorAsync, Func<Task> onCompletedAsync)
    {
        OnNextAsync = onNextAsync;
        OnErrorAsync = onErrorAsync;
        OnCompletedAsync = onCompletedAsync;
    }
}