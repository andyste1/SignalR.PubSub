namespace SignalR.PubSub.Demo.Server
{
    using System;

    using Microsoft.Practices.Prism.PubSubEvents;

    using SignalR.PubSub.Common;
    using SignalR.PubSub.Server;

    /// <summary>
    /// Example shim that subscribes to events of type ISignalREvent raised through the Prism
    /// event aggregator by other business logic within the server, and forwards them on to
    /// the SignalR.PubSub framework to be broadcast.
    /// </summary>
    internal class PrismEventAggregatorShim : IServerEventShim
    {
        private readonly IEventAggregator _prismEventAggregator;

        /// <summary>
        /// Gets or sets the broadcast action. This gets set during initialisation of the
        /// SignalR.PubSub mechanism. You do not need to set it yourself.
        /// </summary>
        public Action<ISignalREvent> BroadcastAction { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrismEventAggregatorShim" /> class.
        /// </summary>
        /// <param name="prismEventAggregator">The event aggregator.</param>
        public PrismEventAggregatorShim(IEventAggregator prismEventAggregator)
        {
            _prismEventAggregator = prismEventAggregator;

            // Configure Prism's event aggregator to subscribe to events of type ISignalREvent. (Yes, their syntax is awful).
            _prismEventAggregator.GetEvent<PubSubEvent<ISignalREvent>>().Subscribe(e =>
            {
                // Forward them to the SignalR.PubSub broadcast mechanism.
                if (BroadcastAction != null)
                {
                    BroadcastAction(e);
                }
            });
        }
    }
}
