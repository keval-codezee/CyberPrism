namespace CyberPrism.Server.Constants
{
    public static class ServerConstants
    {
        // Configuration Keys
        public const string ConfigDefaultAppSettings = "DefaultAppSettings";
        public const string ConfigInitialProductionJobs = "InitialProductionJobs";

        // Routes
        public const string RouteApiController = "api/[controller]";

        // Messages
        public const string MsgSettingsUpdated = "Settings updated successfully.";
        public const string MsgJobExists = "Job {0} already exists.";
        public const string MsgJobCreated = "Job {0} created successfully.";

        // Database Tables
        public const string TableDashboardMetrics = "dashboard_metrics";
        public const string TableProductionJobs = "production_jobs";
        public const string TableSensorData = "sensor_data";
        public const string TableAppSettings = "app_settings";

        // Database Columns (Common)
        public const string ColId = "id";
        public const string ColCreatedAt = "created_at";
        public const string ColUpdatedAt = "updated_at";
        public const string ColTimestamp = "timestamp";

        // Database Columns (Dashboard)
        public const string ColProdValue = "production_value";
        public const string ColTargetValue = "target_value";
        public const string ColEfficiency = "efficiency";
        public const string ColQualityPass = "quality_pass";
        public const string ColQualityFail = "quality_fail";
        public const string ColPower = "power_consumption";

        // Database Columns (Production)
        public const string ColJobId = "job_id";
        public const string ColProduct = "product";
        public const string ColQuantity = "quantity";
        public const string ColCompleted = "completed";
        public const string ColStatus = "status";
        public const string ColDueDate = "due_date";

        // Database Columns (Sensor)
        public const string ColSensorId = "sensor_id";
        public const string ColValue = "value";
        public const string ColDataType = "data_type";

        // Database Columns (Settings)
        public const string ColShowDashboard = "show_dashboard";
        public const string ColShowProduction = "show_production";
        public const string ColShowAnalytics = "show_analytics";
        public const string ColShowFactoryVisual = "show_factory_visual";
        public const string ColMachineName = "machine_name";
        public const string ColIpAddress = "ip_address";
        public const string ColIsDarkMode = "is_dark_mode";
        public const string ColNotifications = "notifications_enabled";

        // Database Indexes
        public const string IdxDashboardTimestamp = "idx_dashboard_metrics_timestamp";
        public const string IdxProductionJobId = "idx_production_jobs_job_id";
        public const string IdxProductionStatus = "idx_production_jobs_status";
        public const string IdxProductionDueDate = "idx_production_jobs_due_date";
        public const string IdxSensorTimestamp = "idx_sensor_data_timestamp";
        public const string IdxSensorId = "idx_sensor_data_sensor_id";
        public const string IdxSensorComposite = "idx_sensor_data_sensor_id_timestamp";
    }
}
