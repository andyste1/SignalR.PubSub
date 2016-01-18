namespace SignalR.PubSub.Demo.Client.Net
{
    using System;

    using Microsoft.AspNet.SignalR.Client;
    using Microsoft.Practices.Prism.PubSubEvents;

    using SignalR.PubSub.Client.Net;
    using SignalR.PubSub.Demo.Common;

    /// <summary>
    /// This is an example SignalR .Net client that subscribes to events published on the server.
    /// It uses the Prism event aggregator but you should be able to substitute your own shim(s) 
    /// to interface with your own event aggregator implementation (or even use regular .Net events!).
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            // Instantiate the Prism event aggregator and subscribe to events of type ISignalREvent. (Yes, their syntax is awful).
            var prismEventAggregator = new EventAggregator();
            prismEventAggregator.GetEvent<PubSubEvent<SimpleEvent>>().Subscribe(SimpleEventSubscriber);
            prismEventAggregator.GetEvent<PubSubEvent<ComplexEvent>>().Subscribe(ComplexEventSubscriber);

            // Prepare the connection.
            var hubConnection = new HubConnection("http://localhost:8700/");
            hubConnection.Error += ex =>
            {
                Console.WriteLine("SignalR error: {0}", ex.Message);
            };

            // Create and initialise our shim. Must be done before hubConnection.Start().
            var clientShim = new PrismEventAggregatorShim(prismEventAggregator);
            PubSubSignalRClient.Initialise(hubConnection, clientShim);

            // Connect.
            hubConnection.Start().Wait();

            Console.WriteLine("Client started! Press any key to stop...");
            Console.ReadKey();
        }

        /// <summary>
        /// Subscribes to SimpleEvents.
        /// </summary>
        /// <param name="simpleEvent">The event.</param>
        private static void SimpleEventSubscriber(SimpleEvent simpleEvent)
        {
            Console.WriteLine("Simple event. Message text: {0}", simpleEvent.MessageText);
            Console.WriteLine();
        }

        /// <summary>
        /// Event subscriber delegate. Simply outputs details of the received events to the console.
        /// </summary>
        /// <param name="complexEvent">The event.</param>
        private static void ComplexEventSubscriber(ComplexEvent complexEvent)
        {
            Console.WriteLine("Complex event. Employee: {0} {1}", complexEvent.Employee.Name, complexEvent.Employee.BirthDate);
            Console.WriteLine("               Timestamp: {0}", complexEvent.Timestamp);
            Console.WriteLine();
        }
    }
}
