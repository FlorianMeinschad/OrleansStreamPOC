using Orleans;
using Streams.Models.Interfaces;

namespace Streams.Models.MessageBus;

/// <summary>
/// Internal wrapper class to encapsulate delegate, because orleans cannot serialize Func<>.
/// </summary>
[GenerateSerializer]
internal class UnsubscribeHandler(Func<Task> unsubscribeMethod) : IUnsubscribe
{
    public async Task UnsubscribeAsync()
    {
        await unsubscribeMethod();
    }
}