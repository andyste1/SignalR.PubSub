namespace SignalR.PubSub.Demo.Server
{
    using System;

    using Microsoft.Owin.Hosting;
    using Microsoft.Practices.Prism.PubSubEvents;

    using SignalR.PubSub.Server;

    /// <summary>
    /// This is a self-hosted SignalR server containing a simple business logic class that
    /// publishes events via Prism's event aggregator.
    /// You should be able to substitute your own shim(s) to interface with your own
    /// event aggregator implementation (or even use regular .Net events!).
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            // Create the Prism event aggregator.
            var prismEventAggregator = new EventAggregator();

            // If you get an access denied error when running the demo, try this command line:
            //     netsh http add urlacl url=http://+:8700/MyUri user=DOMAIN\user
            const string Url = "http://+:8700/";

            // Register shims before WebApp.Start(). This ensures that the library's internal
            // Hub is visible to the SignalR server.
            PubSubSignalRServer.RegisterShims(new[]
                                          {
                                              new PrismEventAggregatorShim(prismEventAggregator), 
                                          });

            using (WebApp.Start<OwinConfig>(Url))
            {
                // Fire up the test BLL that will use the event service to broadcast to client.
                var myBusinessLogic = new MyBusinessLogic(prismEventAggregator);

                Console.WriteLine("Server started. Press any key to stop...");
                Console.ReadKey();
            }
        }
    }
}
