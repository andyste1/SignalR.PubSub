namespace SignalR.PubSub.Client.Net
{
    using SignalR.PubSub.Common;

    /// <summary>
    /// Client-side shim that sits between SignalR and the client applications own event mechanism
    /// (an event aggregator library, for example).
    /// </summary>
    public interface IClientEventShim
    {
        /// <summary>
        /// Publishes the specified event, typically via the application's own event
        /// aggregator, or other eventing mechanism.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <param name="event">The event.</param>
        void Publish<TEvent>(TEvent @event) where TEvent : ISignalREvent;
    }
}
