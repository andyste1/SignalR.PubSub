namespace SignalR.PubSub.Demo.Common
{
    using System;

    using SignalR.PubSub.Common;

    /// <summary>
    /// An event containing a complex property type.
    /// </summary>
    public class ComplexEvent : ISignalREvent
    {
        public Employee Employee { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class Employee
    {
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
