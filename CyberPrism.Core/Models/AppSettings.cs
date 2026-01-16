using Prism.Mvvm;

namespace CyberPrism.Core.Models
{
    public class AppSettings : BindableBase
    {
        private bool _showDashboard = true;
        public bool ShowDashboard
        {
            get { return _showDashboard; }
            set { SetProperty(ref _showDashboard, value); }
        }

        private bool _showProduction = true;
        public bool ShowProduction
        {
            get { return _showProduction; }
            set { SetProperty(ref _showProduction, value); }
        }

        private bool _showAnalytics = true;
        public bool ShowAnalytics
        {
            get { return _showAnalytics; }
            set { SetProperty(ref _showAnalytics, value); }
        }

        private bool _showFactoryVisual = true;
        public bool ShowFactoryVisual
        {
            get { return _showFactoryVisual; }
            set { SetProperty(ref _showFactoryVisual, value); }
        }

        private string _machineName = "Machine-001";
        public string MachineName
        {
            get { return _machineName; }
            set { SetProperty(ref _machineName, value); }
        }

        private string _ipAddress = "192.168.1.100";
        public string IpAddress
        {
            get { return _ipAddress; }
            set { SetProperty(ref _ipAddress, value); }
        }

        private bool _isDarkMode = true;
        public bool IsDarkMode
        {
            get { return _isDarkMode; }
            set { SetProperty(ref _isDarkMode, value); }
        }

        private bool _notificationsEnabled = true;
        public bool NotificationsEnabled
        {
            get { return _notificationsEnabled; }
            set { SetProperty(ref _notificationsEnabled, value); }
        }
        public override bool Equals(object? obj)
        {
            if (obj is not AppSettings other) return false;

            return ShowDashboard == other.ShowDashboard &&
                   ShowProduction == other.ShowProduction &&
                   ShowAnalytics == other.ShowAnalytics &&
                   ShowFactoryVisual == other.ShowFactoryVisual &&
                   MachineName == other.MachineName &&
                   IpAddress == other.IpAddress &&
                   IsDarkMode == other.IsDarkMode &&
                   NotificationsEnabled == other.NotificationsEnabled;
        }

        public override int GetHashCode()
        {
            return (ShowDashboard, ShowProduction, ShowAnalytics, ShowFactoryVisual, MachineName, IpAddress, IsDarkMode, NotificationsEnabled).GetHashCode();
        }

        public AppSettings Clone()
        {
            return new AppSettings
            {
                ShowDashboard = this.ShowDashboard,
                ShowProduction = this.ShowProduction,
                ShowAnalytics = this.ShowAnalytics,
                ShowFactoryVisual = this.ShowFactoryVisual,
                MachineName = this.MachineName,
                IpAddress = this.IpAddress,
                IsDarkMode = this.IsDarkMode,
                NotificationsEnabled = this.NotificationsEnabled
            };
        }
    }
}

