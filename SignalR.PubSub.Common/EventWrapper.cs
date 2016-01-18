namespace SignalR.PubSub.Common
{
    using System;

    /// <summary>
    /// Wraps an ISignalREvent to preserve the concrete type during serialization.
    /// </summary>
    public class EventWrapper
    {
        /// <summary>
        /// Gets or sets the event's type.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Gets or sets the serialized JSON.
        /// </summary>
        public string Json { get; set; }
    }
}