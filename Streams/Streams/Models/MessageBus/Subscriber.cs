namespace Streams.Models.MessageBus;

internal record Subscriber<T>(Func<T, Task> Callback)
{
    public Guid Id { get; } = Guid.NewGuid();
}