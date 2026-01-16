namespace CyberPrism.Core.Constants
{
    public static class AppConstants
    {
        // File Paths & Folders
        public const string AppDataFolderName = "CyberPrism";
        public const string SettingsFileName = "settings.json";

        // API Endpoints
        public const string BaseApiUrl = "http://localhost:5133/api/";
        public const string SettingsEndpoint = "settings";
        public const string DashboardEndpoint = "dashboard";

        // Navigation Views
        public const string RegionContent = "ContentRegion";
        public const string RegionNavigation = "NavigationRegion";
        public const string RegionFactoryVisual = "FactoryVisualRegion";

        public const string DashboardView = "DashboardView";
        public const string SettingsView = "SettingsView";
        public const string ErrorView = "ErrorView";
        public const string ProductionView = "ProductionView";
        public const string AnalyticsView = "AnalyticsView";

        // UI Strings
        public const string SystemOperational = "System Operational";
        public const string SystemOffline = "System Offline";
        public const string ConnectionLostTitle = "CONNECTION LOST";
        public const string ConnectionLostMessage = "Manufacturing Server Unreachable";
        public const string ConnectionRetryMessage = "Retrying connection automatically...";

        public const string ProductionTitle = "Production";
        public const string TargetTitle = "Target";
        public const string EfficiencyTitle = "Efficiency";
        public const string PassTitle = "Pass";
        public const string FailTitle = "Fail";
        public const string PowerTitle = "kW/h";
        public const string DashboardTitle = "Dashboard";

        public const string MonthJan = "Jan";
        public const string MonthFeb = "Feb";
        public const string MonthMar = "Mar";
        public const string MonthApr = "Apr";
        public const string MonthMay = "May";

        // Analytics Strings
        public const string AnalyticsTitle = "Analytics";
        public const string AnalyticsEndpoint = "analytics";
        
        public const string AvailabilityTitle = "Availability";
        public const string PerformanceTitle = "Performance";
        public const string QualityTitle = "Quality";
        public const string DowntimeChartTitle = "Downtime (min)";
        public const string UnitMinutes = " min";

        public const string DowntimeMechanical = "Mechanical";
        public const string DowntimeElectrical = "Electrical";
        public const string DowntimeMaterial = "Material";
        public const string DowntimeOperator = "Operator";
        public const string DowntimeOther = "Other";

        public const string DataTypeOee = "OEE";
        public const string DataTypeDowntime = "Downtime";
        public const string SensorPrefix = "Sensor-";
        public const string SeverityHigh = "High";
    }
}
