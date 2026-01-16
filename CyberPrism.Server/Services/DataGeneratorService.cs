using Microsoft.EntityFrameworkCore;
using CyberPrism.Server.Data;
using CyberPrism.Server.Models;
using CyberPrism.Server.Configuration;

namespace CyberPrism.Server.Services
{
    /// <summary>
    /// Background worker that simulates real-world industrial data generation.
    /// It runs periodically to populate the database with metrics, sensor data, and job progress.
    /// </summary>
    public class DataGeneratorService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DataGeneratorService> _logger;
        private readonly Random _random = new Random();

        /// <summary>
        /// Initializes a new instance of DataGeneratorService.
        /// </summary>
        /// <param name="serviceProvider">Used to create scopes for resolving scoped services like DbContext.</param>
        /// <param name="logger">Standard logger for service activity monitoring.</param>
        public DataGeneratorService(IServiceProvider serviceProvider, ILogger<DataGeneratorService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <summary>
        /// The main execution loop for the background service.
        /// </summary>
        /// <param name="stoppingToken">Triggered when the application host is shutting down.</param>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Data Generator Service is starting...");

            // Brief delay to allow the DB and Host to stabilize
            await Task.Delay(2000, stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Generate three types of simulated data in each tick
                    await GenerateDashboardMetricsAsync();
                    await GenerateSensorDataAsync();
                    await UpdateProductionJobsAsync();

                    _logger.LogDebug("Generated new batch of simulated data");

                    // Hibernate until the next generation cycle as defined in configuration
                    await Task.Delay(AppConstants.DataGeneration.GenerationIntervalMs, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while generating data. Retrying in 10s.");
                    await Task.Delay(10000, stoppingToken);
                }
            }

            _logger.LogInformation("Data Generator Service is stopping...");
        }

        /// <summary>
        /// Generates random KPI metrics for the dashboard (OEE, Power, etc.).
        /// </summary>
        private async Task GenerateDashboardMetricsAsync()
        {
            // Scoped services like DbContext must be created via a scope in background workers
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IndustrialDbContext>();

            // Create a randomized metric entry based on pre-configured ranges
            var metric = new DashboardMetricEntity
            {
                ProductionValue = _random.Next(AppConstants.RandomRanges.ProductionValueMin, AppConstants.RandomRanges.ProductionValueMax),
                TargetValue = _random.Next(AppConstants.RandomRanges.TargetValueMin, AppConstants.RandomRanges.TargetValueMax),
                Efficiency = _random.Next(AppConstants.RandomRanges.EfficiencyMin, AppConstants.RandomRanges.EfficiencyMax),
                QualityPass = _random.Next(AppConstants.RandomRanges.QualityPassMin, AppConstants.RandomRanges.QualityPassMax),
                QualityFail = _random.Next(AppConstants.RandomRanges.QualityFailMin, AppConstants.RandomRanges.QualityFailMax),
                PowerConsumption = _random.Next(AppConstants.RandomRanges.PowerConsumptionMin, AppConstants.RandomRanges.PowerConsumptionMax),
                Timestamp = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await context.DashboardMetrics.AddAsync(metric);
            await context.SaveChangesAsync();

            // Self-maintenance: Remove old metrics to prevent the database from growing indefinitely
            var oldMetrics = await context.DashboardMetrics
                .OrderByDescending(m => m.Timestamp)
                .Skip(AppConstants.DataGeneration.DashboardMetricsRetentionCount)
                .ToListAsync();

            if (oldMetrics.Any())
            {
                context.DashboardMetrics.RemoveRange(oldMetrics);
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Simulates a batch of raw sensor readings from various sensors.
        /// </summary>
        private async Task GenerateSensorDataAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IndustrialDbContext>();

            var sensorDataPoints = new List<SensorDataEntity>();

            // Generate a burst of sensor readings
            for (int i = 0; i < AppConstants.DataGeneration.SensorDataPointsPerBatch; i++)
            {
                sensorDataPoints.Add(new SensorDataEntity
                {
                    Timestamp = DateTime.UtcNow,
                    // Randomly assign to a sensor (e.g., Sensor-1 to Sensor-N)
                    SensorId = $"Sensor-{_random.Next(1, AppConstants.Sensors.SensorCount + 1)}",
                    Value = _random.NextDouble() * AppConstants.RandomRanges.SensorValueMax,
                    // Randomly select a data type (e.g., "OEE", "Downtime")
                    DataType = AppConstants.Sensors.DataTypes[_random.Next(AppConstants.Sensors.DataTypes.Length)],
                    CreatedAt = DateTime.UtcNow
                });
            }

            await context.SensorData.AddRangeAsync(sensorDataPoints);
            await context.SaveChangesAsync();

            // Retain only the most recent N sensor data points
            var oldSensorData = await context.SensorData
                .OrderByDescending(s => s.Timestamp)
                .Skip(AppConstants.DataGeneration.SensorDataRetentionCount)
                .ToListAsync();

            if (oldSensorData.Any())
            {
                context.SensorData.RemoveRange(oldSensorData);
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Advances the progress of all 'Running' production jobs.
        /// </summary>
        private async Task UpdateProductionJobsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IndustrialDbContext>();

            // Fetch jobs that are actively being worked on
            var runningJobs = await context.ProductionJobs
                .Where(j => j.Status == "Running")
                .ToListAsync();

            foreach (var job in runningJobs)
            {
                if (job.Completed < job.Quantity)
                {
                    // Increment completed units by a random amount
                    var increment = _random.Next(AppConstants.Production.MinProgressIncrement, AppConstants.Production.MaxProgressIncrement);
                    job.Completed = Math.Min(job.Completed + increment, job.Quantity);

                    // Automatic status transition to Completed
                    if (job.Completed >= job.Quantity)
                    {
                        job.Status = "Completed";
                    }

                    job.UpdatedAt = DateTime.UtcNow;
                }
            }

            // Batch save all job updates
            if (runningJobs.Any())
            {
                await context.SaveChangesAsync();
            }
        }
    }
}
