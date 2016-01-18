# SignalR.PubSub

A simple SignalR-based event aggregator that allows clients to subscribe to server events.

### The Problem
You use some kind of event aggregator framework (or even "conventional" events) to publish server-side events, and you want to subscribe to these on the client side. Normally you would probably do something like this: 
* Server-side business logic publishes/raises an event
* Server-side hub subscribes/handles the event and broadcasts it to clients using *Clients.All...*
* Client-side subscribes to this SignalR publication using *_hubProxy.On<>* and uses delegate to publish/raise the event on the client-side

Needing to handle more than a couple of events can quickly result in a lot of code repetition.

By the way, if you don't currently use an event aggregator library then you might want to read the later section titled *But I don't use an Event Aggregator!*

### The Solution
This is where the SignalR.PubSub library comes in. It hooks in to your existing event aggregator framework and forwards server-side events to any connected clients. It's still performing the above steps under the covers, but it's all taken care of for you - publish events on the server-side and subscribe to them on the client-side, using your favourite event aggregator (or conventional events).

# How to use SignalR.PubSub
Your "event" classes will need to reside in a separate project, as they'll be referenced by both the client- and server-side projects. These classes must implement the *ISignalREvent* interface. There are no properties or methods to implement - it's just a "marker interface" used to indicate which events published by server-side business logic should be forwarded to clients:

```C#
using SignalR.PubSub.Common;

public class ExchangeRateChangedEvent : ISignalREvent 
{ 
   public string FromCurrencySymbol {get; set;}
   public string ToCurrencySymbol {get; set;}
   public decimal NewExchangeRate { get; set; }
}
```

### Server-Side
On the server-side, you'll need to create a small "shim" class that acts as a go-between, subscribing to events published by the event aggregator, and forwarding them on to SignalR.PubSub (for broadcasting to clients). Below is an example shim based on Prism's EventAggregator class:-

```C#
internal class PrismEventAggregatorShim : IServerEventShim
{
    private readonly IEventAggregator _prismEventAggregator;

    public Action<ISignalREvent> BroadcastAction { get; set; }

    public PrismEventAggregatorShim(IEventAggregator prismEventAggregator)
    {
        _prismEventAggregator = prismEventAggregator;
        _prismEventAggregator.GetEvent<PubSubEvent<ISignalREvent>>().Subscribe(e =>
        {
            if (BroadcastAction != null)
            {
                BroadcastAction(e);
            }
        });
    }
}
```
The class should implement the *IServerEventShim* interface. In the above example, the Prism event aggregator is passed to the constructor, which subscribes to events of type *ISignalREvent* (I know, their syntax isn't very nice). The subscriber delegate simply passes the event object to the *BroadcastAction* Action delegate (this is populated when the shim is registered - see below).

Finally, you need to register your shim. Typically you'll need to do this before SignalR starts up - in Global.asax, or before WebApp.Start<> (if using self-hosting):-

```C#
PubSubSignalRServer.RegisterShims(new[]
      {
          new PrismEventAggregatorShim(prismEventAggregator), 
      });
```
(The method accepts a collection of IServerEventShim objects to support more advanced scenarios, but normally you should only need one shim).

### Client-Side (.Net Clients)
The client-side needs a shim too. This time it's to take events received from the server (via SignalR) and forward them on to whichever client-side event aggregator is being used. Again, this example uses Prism's EventAggregator:-

```C#
internal class PrismEventAggregatorShim : IClientEventShim
{
    private readonly IEventAggregator _prismEventAggregator;

    public PrismEventAggregatorShim(IEventAggregator prismEventAggregator)
    {
        _prismEventAggregator = prismEventAggregator;
    }

    public void Publish<TEvent>(TEvent @event) where TEvent : ISignalREvent
    {
        _prismEventAggregator.GetEvent<PubSubEvent<TEvent>>().Publish(@event);
    }
}
```
The class implements the *IClientEventShim* interface and requires you to implement the *Publish()* method where you publish the event to the client's event aggregator. In the above example we are again using Prism's event aggregator (which again uses a convoluted syntax!).

Once again, you need to register the shim during startup. This needs to happen after the HubConnection has been created but before it has been started:-

```C#
var hubConnection = new HubConnection("http://localhost:8700/");
...
var clientShim = new PrismEventAggregatorShim(_eventAggregator);
PubSubSignalRClient.Initialise(hubConnection, clientShim);
...
await hubConnection.Start();
```
Elsewhere in the client application you simply subscribe to events via your event aggregator of choice, in the usual way (again, Prism is used in this example):-

```C#
public class HomeViewModel
{
   private readonly IEventAggregator _eventAggregator;
   
   public HomeViewModel(IEventAggregator eventAggregator)
   {
        _eventAggregator = eventAggregator;
        _eventAggregator.GetEvent<PubSubEvent<ExchangeRateChangedEvent>>().Subscribe(SubscriberDelegate);
        );
   }
   
   private void SubscriberDelegate(ExchangeRateChangedEvent @event)
   {
        ...
   }
}
```

You can find a demo solution in this GitHub repo that contains a self-hosted SignalR server and a .Net client.

### Javascript Clients
While I haven't added any official support for JS clients, it *is* still possible to use this library. The SignalR.PubSub library utilises its own "hidden" Hub through which server-side events are broadcast to clients. The object that is passed across the connection is a "wrapper" that contains the event object's JSON, and the event object's Type. You can utilise this wrapper object in Javascript, something like this:-

```JS
var connection = $.hubConnection("http://localhost:8700/signalr", { useDefaultPath: false });
var hubProxy = connection.createHubProxy('_PubSubHub');

hubProxy.on('PubSubBusEvent', function (message) {
    var eventObject = $.parseJSON(message.Json);
    if (message.Type === 'MyNamespace.ExchangeRateChangedEvent') {
        console.log('   New exchange rate: ' + eventObject.NewExchangeRate);
    }
});
```

Make sure you use the correct Hub name ("_PubSubHub") and Hub event name ("PubSubBusEvent") as shown above.

# But I don't use an Event Aggregator!
Why not?! Event aggregators are great for decoupling publishers (classes that raise events) from subscribers (classes that handle events). A publisher doesn't care who (if anyone) is subscribing to its events; similarly a subscriber doesn't know or care who published that event. We all know that decoupling promotes better OO design, and can help make classes easier to unit test. 

While this library wasn't designed with "conventional" events in mind, it *is* possible to handle them - just. If you do still want to continue down this route, read on...

### Server-Side
You'll need a shim that handles an event raised in your business logic class and forwards it on to SignalR for broadcasting to clients. For example:-

```C#
internal class EventShim : IServerEventShim
{
    private MyBusinessLogic _myBusinessLogic;

    public Action<ISignalREvent> BroadcastAction { get; set; }

    public EventShim(MyBusinessLogic myBusinessLogic)
    {
        _myBusinessLogic = myBusinessLogic;
        _myBusinessLogic.SomeEvent += (sender, args) =>
                if (BroadcastAction != null)
                    BroadcastAction(args);
    }
}
```

You'll probably want to create a separate shim class for each event (or at least each class that raises events). Go ahead and register the shim(s) as described earlier - remember when I mentioned that you can register more than one shim?

As you are using conventional events, your event classes should inherit from EventArgs but must still implement *ISignalREvent*.

### Client-Side
On the client-side, the shim needs to take the event that has arrived via SignalR, and raise a "conventional" event, something like this:-

```C#
internal class EventShim : IClientEventShim
{
    public event EventHandler<ExchangeRateChangedEventArgs> ExchangeRateChanged;

    internal class EventShim : IClientEventShim
    {
        public event EventHandler<ExchangeRateChangedEventArgs> ExchangeRateChanged;

        public void Publish<TEvent>(TEvent @event) where TEvent : ISignalREvent
        {
            if (typeof(TEvent) == typeof(ExchangeRateChangedEventArgs) && ExchangeRateChanged != null)
            {
                ExchangeRateChanged(this, @event as ExchangeRateChangedEventArgs);
            }
            if (typeof(TEvent) == ....
        }
    }
}
```

The problem here is that the Publish() event is generic so you'll have to examine the type and raise the appropriate event. Additionally there can only be one client-side shim so this could get messy if you need to support a number of events. You'll also need to find a way to pass this event shim instance around where it's required, for consumers to set up their event handlers. Are you sure you don't want to look into event aggregators?!
