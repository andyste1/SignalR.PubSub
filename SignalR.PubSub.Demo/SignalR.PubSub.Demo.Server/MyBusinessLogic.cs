namespace SignalR.PubSub.Demo.Server
{
    using System;
    using System.Threading;

    using Microsoft.Practices.Prism.PubSubEvents;

    using SignalR.PubSub.Common;
    using SignalR.PubSub.Demo.Common;

    /// <summary>
    /// Example business logic that periodically publishes events using Prism's EventAggregator.
    /// Other than referencing ISignalREvent, this code does not use any functionality in the
    /// SignalR.PubSub library. It's all done through the "shim".
    /// </summary>
    public class MyBusinessLogic
    {
        private readonly IEventAggregator _prismEventAggregator;
        private Timer _timer;
        private bool _flip;

        /// <summary>
        /// Initializes a new instance of the <see cref="MyBusinessLogic" /> class.
        /// </summary>
        /// <param name="prismEventAggregator">The event aggregator.</param>
        public MyBusinessLogic(IEventAggregator prismEventAggregator)
        {
            _prismEventAggregator = prismEventAggregator;
            _timer = new Timer(TimerCallback, null, 1000, 1000);
        }

        /// <summary>
        /// Timer callback delegate.
        /// </summary>
        /// <param name="state">The state.</param>
        private void TimerCallback(object state)
        {
            // Every second alternately publish one of our two event types.
            ISignalREvent myEvent;
            if (_flip)
            {
                myEvent = new SimpleEvent
                          {
                              MessageText = string.Format("Server says hi at {0}", DateTime.Now)
                          };
            }
            else
            {
                var employee = new Employee
                               {
                                   Name = "John Smith", 
                                   BirthDate = DateTime.Today
                               };
                myEvent = new ComplexEvent
                          {
                              Employee = employee,
                              Timestamp = DateTime.Now
                          };
            }

            // Publish the event via Prism's event aggregator (horrible syntax!)
            _prismEventAggregator.GetEvent<PubSubEvent<ISignalREvent>>().Publish(myEvent); 

            _flip = !_flip;
        }
    }
}
