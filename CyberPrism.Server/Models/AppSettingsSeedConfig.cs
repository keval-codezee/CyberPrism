namespace CyberPrism.Server.Models
{
    /// <summary>
    /// Configuration model for default app settings seed data
    /// </summary>
    public class AppSettingsSeedConfig
    {
        public bool ShowDashboard { get; set; } = true;
        public bool ShowProduction { get; set; } = true;
        public bool ShowAnalytics { get; set; } = true;
        public bool ShowFactoryVisual { get; set; } = true;
        public string MachineName { get; set; } = "CyberPrism-001";
        public string IpAddress { get; set; } = "127.0.0.1";
        public bool IsDarkMode { get; set; } = true;
        public bool NotificationsEnabled { get; set; } = true;
    }
}

