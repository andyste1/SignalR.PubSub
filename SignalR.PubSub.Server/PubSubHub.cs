namespace SignalR.PubSub.Server
{
    using Microsoft.AspNet.SignalR;
    using Microsoft.AspNet.SignalR.Hubs;

    using SignalR.PubSub.Common;

    /// <summary>
    /// Hub used for broadcasting events to clients.
    /// </summary>
    [HubName(Constants.HubName)]
    public class PubSubHub : Hub
    {
    }
}
