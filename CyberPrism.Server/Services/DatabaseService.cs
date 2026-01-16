using Microsoft.EntityFrameworkCore;
using CyberPrism.Core.Models;
using CyberPrism.Server.Data;
using CyberPrism.Server.Models;
using CyberPrism.Server.Configuration;

namespace CyberPrism.Server.Services
{
    /// <summary>
    /// Service layer for database operations, abstracting EF Core interactions.
    /// </summary>
    public class DatabaseService
    {
        private readonly IndustrialDbContext _context;

        /// <summary>
        /// Initializes a new instance of DatabaseService with the specified database context.
        /// </summary>
        /// <param name="context">The EF Core context.</param>
        public DatabaseService(IndustrialDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Aggregates and returns core metrics for the dashboard view.
        /// </summary>
        /// <returns>A DashboardData object containing trends and current metrics.</returns>
        public async Task<DashboardData> GetDashboardDataAsync()
        {
            // Fetch recent historical metrics from the database
            var recentMetrics = await _context.DashboardMetrics
                .OrderByDescending(m => m.Timestamp)
                .Take(AppConstants.DataGeneration.DashboardMetricsQueryCount)
                .ToListAsync();

            // Return safe defaults if no data exists yet
            if (!recentMetrics.Any())
            {
                return new DashboardData
                {
                    ProductionValues = new List<double>(),
                    TargetValues = new List<double>(),
                    EfficiencyValues = new List<double>(),
                    QualityPass = 0,
                    QualityFail = 0,
                    PowerConsumptionValues = new List<double>()
                };
            }

            // Map database entities to the shared DTO (Data Transfer Object)
            // Reversing ensures the data flows chronologically on the chart
            return new DashboardData
            {
                ProductionValues = recentMetrics.Select(m => m.ProductionValue).Reverse().ToList(),
                TargetValues = recentMetrics.Select(m => m.TargetValue).Reverse().ToList(),
                EfficiencyValues = recentMetrics.Select(m => m.Efficiency).Reverse().ToList(),
                QualityPass = recentMetrics.First().QualityPass,
                QualityFail = recentMetrics.First().QualityFail,
                PowerConsumptionValues = recentMetrics.Select(m => m.PowerConsumption).Reverse().ToList()
            };
        }

        /// <summary>
        /// Retrieves all production jobs, sorted by creation date (newest first).
        /// </summary>
        /// <returns>A list of ProductionJob DTOs.</returns>
        public async Task<List<ProductionJob>> GetProductionJobsAsync()
        {
            var entities = await _context.ProductionJobs
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();

            // Convert persistent entities to bindable job models
            return entities.Select(e => new ProductionJob
            {
                JobId = e.JobId,
                Product = e.Product,
                Quantity = e.Quantity,
                Completed = e.Completed,
                Status = e.Status,
                DueDate = e.DueDate
            }).ToList();
        }

        /// <summary>
        /// Finds a specific production job by its unique JobId.
        /// </summary>
        /// <param name="jobId">The unique job identifier.</param>
        /// <returns>The found entity or null.</returns>
        public async Task<ProductionJobEntity?> GetProductionJobByIdAsync(string jobId)
        {
            return await _context.ProductionJobs
                .FirstOrDefaultAsync(j => j.JobId == jobId);
        }

        /// <summary>
        /// Creates a new production job record in the database.
        /// </summary>
        /// <param name="job">The job details from the client.</param>
        /// <returns>The created persistent entity.</returns>
        public async Task<ProductionJobEntity> CreateProductionJobAsync(ProductionJob job)
        {
            var entity = new ProductionJobEntity
            {
                JobId = job.JobId,
                Product = job.Product,
                Quantity = job.Quantity,
                Completed = job.Completed,
                Status = job.Status,
                DueDate = job.DueDate,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Stage and commit the new entity
            _context.ProductionJobs.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Updates an existing production job's persistent state.
        /// </summary>
        /// <param name="job">The modified entity tracking changes.</param>
        public async Task UpdateProductionJobAsync(ProductionJobEntity job)
        {
            job.UpdatedAt = DateTime.UtcNow;
            _context.ProductionJobs.Update(job);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves a set of recent sensor data points for analytics.
        /// </summary>
        /// <param name="count">The number of recent samples to fetch.</param>
        /// <returns>A list of sensor data points.</returns>
        public async Task<List<SensorDataPoint>> GetAnalyticsDataAsync(int count = 10)
        {
            var entities = await _context.SensorData
                .OrderByDescending(s => s.Timestamp)
                .Take(count)
                .ToListAsync();

            return entities.Select(e => new SensorDataPoint
            {
                Timestamp = e.Timestamp,
                SensorId = e.SensorId,
                Value = e.Value,
                DataType = e.DataType
            }).ToList();
        }

        /// <summary>
        /// Persists a single sensor reading to the database.
        /// </summary>
        /// <param name="data">The sensor data entity.</param>
        public async Task SaveSensorDataAsync(SensorDataEntity data)
        {
            _context.SensorData.Add(data);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Queries sensor data filtered by type and optional time range.
        /// </summary>
        /// <param name="dataType">The category of sensor data (e.g., "Downtime").</param>
        /// <param name="startTime">Optional start filter.</param>
        /// <param name="endTime">Optional end filter.</param>
        /// <returns>Filtered list of data points.</returns>
        public async Task<List<SensorDataPoint>> GetSensorDataByTypeAsync(string dataType, DateTime? startTime = null, DateTime? endTime = null)
        {
            // Build a dynamic query using IQueryable
            var query = _context.SensorData.AsQueryable();

            query = query.Where(s => s.DataType == dataType);

            if (startTime.HasValue)
                query = query.Where(s => s.Timestamp >= startTime.Value);

            if (endTime.HasValue)
                query = query.Where(s => s.Timestamp <= endTime.Value);

            // Execute the query on the server side
            var entities = await query
                .OrderBy(s => s.Timestamp)
                .ToListAsync();

            return entities.Select(e => new SensorDataPoint
            {
                Timestamp = e.Timestamp,
                SensorId = e.SensorId,
                Value = e.Value,
                DataType = e.DataType
            }).ToList();
        }

        /// <summary>
        /// Fetches the application configuration settings from the database.
        /// </summary>
        /// <returns>The current AppSettings model.</returns>
        public async Task<AppSettings> GetSettingsAsync()
        {
            var entity = await _context.AppSettings.FirstOrDefaultAsync();

            // Default settings if the database hasn't been seeded (fallback)
            if (entity == null)
            {
                return new AppSettings();
            }

            return new AppSettings
            {
                ShowDashboard = entity.ShowDashboard,
                ShowProduction = entity.ShowProduction,
                ShowAnalytics = entity.ShowAnalytics,
                ShowFactoryVisual = entity.ShowFactoryVisual,
                MachineName = entity.MachineName,
                IpAddress = entity.IpAddress,
                IsDarkMode = entity.IsDarkMode,
                NotificationsEnabled = entity.NotificationsEnabled
            };
        }

        /// <summary>
        /// Updates or creates the global application settings.
        /// </summary>
        /// <param name="settings">The new settings state from the client.</param>
        public async Task UpdateSettingsAsync(AppSettings settings)
        {
            var entity = await _context.AppSettings.FirstOrDefaultAsync();

            // Create entry if none exists
            if (entity == null)
            {
                entity = new AppSettingsEntity { Id = 1 };
                _context.AppSettings.Add(entity);
            }

            // Map client model back to entity
            entity.ShowDashboard = settings.ShowDashboard;
            entity.ShowProduction = settings.ShowProduction;
            entity.ShowAnalytics = settings.ShowAnalytics;
            entity.ShowFactoryVisual = settings.ShowFactoryVisual;
            entity.MachineName = settings.MachineName;
            entity.IpAddress = settings.IpAddress;
            entity.IsDarkMode = settings.IsDarkMode;
            entity.NotificationsEnabled = settings.NotificationsEnabled;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}
