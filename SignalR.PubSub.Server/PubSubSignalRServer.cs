namespace SignalR.PubSub.Server
{
    using System.Collections.Generic;

    using Microsoft.AspNet.SignalR;

    using Newtonsoft.Json;

    using SignalR.PubSub.Common;

    /// <summary>
    /// This class is responsible for forwarding events raised by the server to connected clients.
    /// </summary>
    public class PubSubSignalRServer
    {
        private static readonly IHubContext Context;

        /// <summary>
        /// Initializes static members of the <see cref="PubSubSignalRServer"/> class.
        /// </summary>
        static PubSubSignalRServer()
        {
            Context = GlobalHost.ConnectionManager.GetHubContext(Constants.HubName);
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="PubSubSignalRServer"/> class from being created.
        /// </summary>
        private PubSubSignalRServer()
        {
        }

        /// <summary>
        /// Registers the shims.
        /// </summary>
        /// <param name="shims">The shims.</param>
        public static void RegisterShims(IEnumerable<IServerEventShim> shims)
        {
            foreach (var shim in shims)
            {
                shim.BroadcastAction = Broadcast;
            }
        }

        /// <summary>
        /// Broadcasts the specified SignalR event to clients. Can also be called directly rather 
        /// than via any shims.
        /// </summary>
        /// <param name="event">The event to broadcast.</param>
        public static void Broadcast(ISignalREvent @event)
        {
            var eventWrapper = new EventWrapper
                               {
                                   Type = @event.GetType(),
                                   Json = JsonConvert.SerializeObject(@event)
                               };

            Context.Clients.All.PubSubBusEvent(eventWrapper);
        }
    }
}
