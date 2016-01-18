namespace SignalR.PubSub.Demo.Client.Net
{
    using System;

    using Microsoft.Practices.Prism.PubSubEvents;

    using SignalR.PubSub.Client.Net;
    using SignalR.PubSub.Common;
    using SignalR.PubSub.Demo.Common;

    /// <summary>
    /// Example shim that forwards events arriving from the server on to the Prism Event Aggregator.
    /// Code elsewhere in the application would subscribe to these events.
    /// </summary>
    internal class PrismEventAggregatorShim : IClientEventShim
    {
        private readonly IEventAggregator _prismEventAggregator;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrismEventAggregatorShim"/> class.
        /// </summary>
        /// <param name="prismEventAggregator">The prism event aggregator.</param>
        public PrismEventAggregatorShim(IEventAggregator prismEventAggregator)
        {
            _prismEventAggregator = prismEventAggregator;
        }

        /// <summary>
        /// Publishes the specified event, typically via the application's own event
        /// aggregator, or other eventing mechanism.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <param name="event">The event.</param>
        public void Publish<TEvent>(TEvent @event)
        {
            // Here we publish the event using the Prism event aggregator. Code elsewhere
            // in the client application will subscribe to events of type ISignalREvent
            // (in this demo application it's in Program.cs).
            _prismEventAggregator.GetEvent<PubSubEvent<TEvent>>().Publish(@event);
        }
    }
}
