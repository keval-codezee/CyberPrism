using Prism.Events;

namespace CyberPrism.Core.Events
{
    public class DowntimeAlertMessage
    {
        public string Reason { get; set; }
        public double Duration { get; set; }
        public string Severity { get; set; } // "Low", "Medium", "High"
    }

    public class DowntimeAlertEvent : PubSubEvent<DowntimeAlertMessage>
    {
    }
}

