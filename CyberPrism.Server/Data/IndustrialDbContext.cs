using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using CyberPrism.Server.Models;
using CyberPrism.Server.Constants;

namespace CyberPrism.Server.Data
{
    public class IndustrialDbContext : DbContext
    {
        private readonly IConfiguration? _configuration;

        public IndustrialDbContext(DbContextOptions<IndustrialDbContext> options)
            : base(options)
        {
        }

        public IndustrialDbContext(DbContextOptions<IndustrialDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        // DbSets
        public DbSet<DashboardMetricEntity> DashboardMetrics { get; set; }
        public DbSet<ProductionJobEntity> ProductionJobs { get; set; }
        public DbSet<SensorDataEntity> SensorData { get; set; }
        public DbSet<AppSettingsEntity> AppSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Dashboard Metrics Configuration
            modelBuilder.Entity<DashboardMetricEntity>(entity =>
            {
                entity.HasIndex(e => e.Timestamp)
                    .HasDatabaseName(ServerConstants.IdxDashboardTimestamp);
            });

            // Production Jobs Configuration
            modelBuilder.Entity<ProductionJobEntity>(entity =>
            {
                entity.HasIndex(e => e.JobId)
                    .IsUnique()
                    .HasDatabaseName(ServerConstants.IdxProductionJobId);

                entity.HasIndex(e => e.Status)
                    .HasDatabaseName(ServerConstants.IdxProductionStatus);

                entity.HasIndex(e => e.DueDate)
                    .HasDatabaseName(ServerConstants.IdxProductionDueDate);
            });

            // Sensor Data Configuration
            modelBuilder.Entity<SensorDataEntity>(entity =>
            {
                entity.HasIndex(e => e.Timestamp)
                    .HasDatabaseName(ServerConstants.IdxSensorTimestamp);

                entity.HasIndex(e => e.SensorId)
                    .HasDatabaseName(ServerConstants.IdxSensorId);

                entity.HasIndex(e => new { e.SensorId, e.Timestamp })
                    .HasDatabaseName(ServerConstants.IdxSensorComposite);
            });

            // App Settings Configuration
            modelBuilder.Entity<AppSettingsEntity>(entity =>
            {
               // App Settings Configuration (Schema only)
            });
        }
    }
}

