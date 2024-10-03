namespace Streams.Models.Interfaces;

/// <summary>
/// Interface to encapsulate delegate, because orleans cannot serialize Func<>.
/// </summary>
internal interface IUnsubscribe
{
    Task UnsubscribeAsync();
}