using System;
using System.Collections.Generic;

namespace CyberPrism.Core.Models
{
    /// <summary>
    /// Represents the data required for the Dashboard view, including production trends and quality metrics.
    /// </summary>
    public class DashboardData
    {
        /// <summary>List of historical production values.</summary>
        public List<double> ProductionValues { get; set; } = new();
        /// <summary>List of historical target production values.</summary>
        public List<double> TargetValues { get; set; } = new();
        /// <summary>List of historical efficiency percentages.</summary>
        public List<double> EfficiencyValues { get; set; } = new();
        /// <summary>Total count of products that passed quality control.</summary>
        public double QualityPass { get; set; }
        /// <summary>Total count of products that failed quality control.</summary>
        public double QualityFail { get; set; }
        /// <summary>List of historical power consumption values (e.g., kW/h).</summary>
        public List<double> PowerConsumptionValues { get; set; } = new();
    }

    /// <summary>
    /// Represents a production job or order within the system, supporting data binding.
    /// </summary>
    public class ProductionJob : Prism.Mvvm.BindableBase
    {
        private string _jobId = string.Empty;
        /// <summary>Unique identifier for the production job.</summary>
        public string JobId
        {
            get => _jobId;
            set => SetProperty(ref _jobId, value);
        }

        private string _product = string.Empty;
        /// <summary>Name or description of the product being manufactured.</summary>
        public string Product
        {
            get => _product;
            set => SetProperty(ref _product, value);
        }

        private int _quantity;
        /// <summary>The total target quantity for this job.</summary>
        public int Quantity
        {
            get => _quantity;
            set { if (SetProperty(ref _quantity, value)) RaisePropertyChanged(nameof(Progress)); }
        }

        private int _completed;
        /// <summary>The number of units successfully completed so far.</summary>
        public int Completed
        {
            get => _completed;
            set { if (SetProperty(ref _completed, value)) RaisePropertyChanged(nameof(Progress)); }
        }

        private string _status = string.Empty;
        /// <summary>Current status of the job (e.g., "Pending", "Running", "Completed").</summary>
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        private DateTime _dueDate;
        /// <summary>The scheduled completion date for this job.</summary>
        public DateTime DueDate
        {
            get => _dueDate;
            set => SetProperty(ref _dueDate, value);
        }

        /// <summary>Calculated property representing the percentage of completion.</summary>
        public double Progress => Quantity > 0 ? (double)Completed / Quantity * 100 : 0;
    }

    /// <summary>
    /// Data structure for a single sensor reading or data point.
    /// </summary>
    public class SensorDataPoint
    {
        /// <summary>The time when the data was recorded or generated.</summary>
        public DateTime Timestamp { get; set; }
        /// <summary>Identifier for the sensor that produced the data.</summary>
        public string SensorId { get; set; } = string.Empty;
        /// <summary> The numeric value recorded by the sensor.</summary>
        public double Value { get; set; }
        /// <summary>The category of data (e.g., "OEE", "Downtime").</summary>
        public string DataType { get; set; } = string.Empty; 
    }
}
