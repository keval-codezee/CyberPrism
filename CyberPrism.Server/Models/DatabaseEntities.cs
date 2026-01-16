using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CyberPrism.Server.Constants;

namespace CyberPrism.Server.Models
{
    /// <summary>
    /// Dashboard metrics data entity for database storage
    /// </summary>
    [Table(ServerConstants.TableDashboardMetrics)]
    public class DashboardMetricEntity
    {
        [Key]
        [Column(ServerConstants.ColId)]
        public int Id { get; set; }

        [Column(ServerConstants.ColProdValue)]
        public double ProductionValue { get; set; }

        [Column(ServerConstants.ColTargetValue)]
        public double TargetValue { get; set; }

        [Column(ServerConstants.ColEfficiency)]
        public double Efficiency { get; set; }

        [Column(ServerConstants.ColQualityPass)]
        public double QualityPass { get; set; }

        [Column(ServerConstants.ColQualityFail)]
        public double QualityFail { get; set; }

        [Column(ServerConstants.ColPower)]
        public double PowerConsumption { get; set; }

        [Column(ServerConstants.ColTimestamp)]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [Column(ServerConstants.ColCreatedAt)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Production job entity for database storage
    /// </summary>
    [Table(ServerConstants.TableProductionJobs)]
    public class ProductionJobEntity
    {
        [Key]
        [Column(ServerConstants.ColId)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Column(ServerConstants.ColJobId)]
        public string JobId { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        [Column(ServerConstants.ColProduct)]
        public string Product { get; set; } = string.Empty;

        [Column(ServerConstants.ColQuantity)]
        public int Quantity { get; set; }

        [Column(ServerConstants.ColCompleted)]
        public int Completed { get; set; }

        [MaxLength(50)]
        [Column(ServerConstants.ColStatus)]
        public string Status { get; set; } = "Pending";

        [Column(ServerConstants.ColDueDate)]
        public DateTime DueDate { get; set; }

        [Column(ServerConstants.ColCreatedAt)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column(ServerConstants.ColUpdatedAt)]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [NotMapped]
        public double Progress => Quantity > 0 ? (double)Completed / Quantity * 100 : 0;
    }

    /// <summary>
    /// Sensor data entity for time-series storage
    /// </summary>
    [Table(ServerConstants.TableSensorData)]
    public class SensorDataEntity
    {
        [Key]
        [Column(ServerConstants.ColId)]
        public int Id { get; set; }

        [Column(ServerConstants.ColTimestamp)]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(100)]
        [Column(ServerConstants.ColSensorId)]
        public string SensorId { get; set; } = string.Empty;

        [Column(ServerConstants.ColValue)]
        public double Value { get; set; }

        [Required]
        [MaxLength(50)]
        [Column(ServerConstants.ColDataType)]
        public string DataType { get; set; } = string.Empty;

        [Column(ServerConstants.ColCreatedAt)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Application settings entity
    /// </summary>
    [Table(ServerConstants.TableAppSettings)]
    public class AppSettingsEntity
    {
        [Key]
        [Column(ServerConstants.ColId)]
        public int Id { get; set; }

        [Column(ServerConstants.ColShowDashboard)]
        public bool ShowDashboard { get; set; } = true;

        [Column(ServerConstants.ColShowProduction)]
        public bool ShowProduction { get; set; } = true;

        [Column(ServerConstants.ColShowAnalytics)]
        public bool ShowAnalytics { get; set; } = true;

        [Column(ServerConstants.ColShowFactoryVisual)]
        public bool ShowFactoryVisual { get; set; } = true;

        [MaxLength(100)]
        [Column(ServerConstants.ColMachineName)]
        public string MachineName { get; set; } = string.Empty;

        [MaxLength(50)]
        [Column(ServerConstants.ColIpAddress)]
        public string IpAddress { get; set; } = string.Empty;

        [Column(ServerConstants.ColIsDarkMode)]
        public bool IsDarkMode { get; set; } = true;

        [Column(ServerConstants.ColNotifications)]
        public bool NotificationsEnabled { get; set; } = true;

        [Column(ServerConstants.ColUpdatedAt)]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
