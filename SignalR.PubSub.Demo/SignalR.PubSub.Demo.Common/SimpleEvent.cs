namespace SignalR.PubSub.Demo.Common
{
    using SignalR.PubSub.Common;

    /// <summary>
    /// A simple event.
    /// </summary>
    public class SimpleEvent : ISignalREvent
    {
        public string MessageText { get; set; }
    }
}
