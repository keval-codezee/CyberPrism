namespace CyberPrism.Modules.Settings.Models
{
    public class MachineSettings
    {
        public string MachineName { get; set; } = "Eclipse Machine 01";
        public string IpAddress { get; set; } = "192.168.1.100";
        public bool IsDarkMode { get; set; } = true;
        public bool NotificationsEnabled { get; set; } = true;
    }
}

