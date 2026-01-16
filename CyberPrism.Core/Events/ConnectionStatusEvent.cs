using Prism.Events;

namespace CyberPrism.Core.Events
{
    /// <summary>
    /// Event broadcast when the connection status to the server changes.
    /// Payload: true if connected, false if disconnected.
    /// </summary>
    public class ConnectionStatusEvent : PubSubEvent<bool>
    {
    }
}

