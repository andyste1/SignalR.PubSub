namespace SignalR.PubSub.Server
{
    using System;

    using SignalR.PubSub.Common;

    /// <summary>
    /// Server-side shim that sits between the eventing mechanism (such as an event aggregator)
    /// and the SignalR broadcast functionality.
    /// </summary>
    public interface IServerEventShim
    {
        /// <summary>
        /// Gets or sets the broadcast action. This gets set during initialisation of the
        /// SignalR.PubSub mechanism. You do not need to set it yourself.
        /// </summary>
        Action<ISignalREvent> BroadcastAction { get; set; }
    }
}
