namespace SignalR.PubSub.Client.Net
{
    using System.Reflection;

    using Microsoft.AspNet.SignalR.Client;

    using Newtonsoft.Json;

    using SignalR.PubSub.Common;

    /// <summary>
    /// Client-side event proxy
    /// </summary>
    public static class PubSubSignalRClient
    {
        private static IHubProxy hubProxy;
        private static IClientEventShim shim;

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="hubConnection">The hub connection.</param>
        /// <param name="clientEventShim">The client event shim.</param>
        public static void Initialise(
            HubConnection hubConnection,
            IClientEventShim clientEventShim)
        {
            hubProxy = hubConnection.CreateHubProxy(Constants.HubName);
            hubProxy.On<EventWrapper>("PubSubBusEvent", EventReceived);

            shim = clientEventShim;
        }

        /// <summary>
        /// Handles an event received from the server.
        /// </summary>
        /// <param name="eventWrapper">The event wrapper.</param>
        private static void EventReceived(EventWrapper eventWrapper)
        {
            var @event = JsonConvert.DeserializeObject(eventWrapper.Json, eventWrapper.Type) as ISignalREvent;
            if (@event == null)
            {
                return;
            }

            // Call the generic Publish<> method using the event type stored in the wrapper.
            var method = shim.GetType().GetMethod("Publish", BindingFlags.Public | BindingFlags.Instance);
            method = method.MakeGenericMethod(eventWrapper.Type);
            method.Invoke(shim, new object[] { @event });
        }
    }
}
