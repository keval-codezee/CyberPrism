namespace CyberPrism.Server.Configuration
{
    /// <summary>
    /// Centralized configuration constants for the industrial automation server
    /// </summary>
    public static class AppConstants
    {
        /// <summary>
        /// Data generation and retention settings
        /// </summary>
        public static class DataGeneration
        {
            /// <summary>
            /// How many dashboard metrics to keep in database
            /// </summary>
            public const int DashboardMetricsRetentionCount = 100;

            /// <summary>
            /// How many sensor data points to keep in database
            /// </summary>
            public const int SensorDataRetentionCount = 1000;

            /// <summary>
            /// Data generation interval in milliseconds (1 seconds)
            /// </summary>
            public const int GenerationIntervalMs = 1000;

            /// <summary>
            /// Number of sensor data points generated per batch
            /// </summary>
            public const int SensorDataPointsPerBatch = 10;

            /// <summary>
            /// Number of recent dashboard metrics to return in API
            /// </summary>
            public const int DashboardMetricsQueryCount = 5;
        }

        /// <summary>
        /// Sensor data configuration
        /// </summary>
        public static class Sensors
        {
            /// <summary>
            /// Available sensor data types
            /// </summary>
            public static readonly string[] DataTypes = new[]
            {
                "OEE",           // Overall Equipment Effectiveness
                "Downtime",      // Downtime tracking
                "Temperature",   // Temperature monitoring
                "Vibration",     // Vibration analysis
                "Pressure"       // Pressure monitoring
            };

            /// <summary>
            /// Number of unique sensors in the system
            /// </summary>
            public const int SensorCount = 5;
        }

        /// <summary>
        /// Production job configuration
        /// </summary>
        public static class Production
        {
            /// <summary>
            /// Minimum progress increment per update cycle
            /// </summary>
            public const int MinProgressIncrement = 5;

            /// <summary>
            /// Maximum progress increment per update cycle
            /// </summary>
            public const int MaxProgressIncrement = 50;
        }

        /// <summary>
        /// Random data generation ranges
        /// </summary>
        public static class RandomRanges
        {
            // Dashboard metrics
            public const int ProductionValueMin = 50;
            public const int ProductionValueMax = 100;
            public const int TargetValueMin = 70;
            public const int TargetValueMax = 90;
            public const int EfficiencyMin = 60;
            public const int EfficiencyMax = 95;
            public const int QualityPassMin = 800;
            public const int QualityPassMax = 1000;
            public const int QualityFailMin = 20;
            public const int QualityFailMax = 100;
            public const int PowerConsumptionMin = 200;
            public const int PowerConsumptionMax = 500;

            // Sensor data
            public const double SensorValueMax = 100.0;

            // Job IDs
            public const int JobIdMin = 1000;
            public const int JobIdMax = 2000;
        }
    }
}


