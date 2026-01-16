using CyberPrism.Server.Data;
using CyberPrism.Server.Models;
using Microsoft.EntityFrameworkCore;
using CyberPrism.Server.Constants;

namespace CyberPrism.Server.Services
{
    /// <summary>
    /// Helper class responsible for ensuring the database exists and seeding it with initial data.
    /// </summary>
    public class DatabaseSeeder
    {
        /// <summary>
        /// Checks if the database is Empty and populates it with default settings and production jobs if necessary.
        /// </summary>
        /// <param name="serviceProvider">The service provider to resolve dependencies.</param>
        /// <param name="configuration">Application configuration containing seed data.</param>
        public static void Initialize(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            using (var context = new IndustrialDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<IndustrialDbContext>>(),
                configuration))
            {
                // Ensure database is created and schema is up to date
                context.Database.EnsureCreated();

                // Seed App Settings if none exist
                if (!context.AppSettings.Any())
                {
                    var defaultSettings = configuration.GetSection(ServerConstants.ConfigDefaultAppSettings).Get<AppSettingsSeedConfig>();
                    if (defaultSettings != null)
                    {
                        context.AppSettings.Add(new AppSettingsEntity
                        {
                            Id = 1,
                            ShowDashboard = defaultSettings.ShowDashboard,
                            ShowProduction = defaultSettings.ShowProduction,
                            ShowAnalytics = defaultSettings.ShowAnalytics,
                            ShowFactoryVisual = defaultSettings.ShowFactoryVisual,
                            MachineName = defaultSettings.MachineName,
                            IpAddress = defaultSettings.IpAddress,
                            IsDarkMode = defaultSettings.IsDarkMode,
                            NotificationsEnabled = defaultSettings.NotificationsEnabled,
                            UpdatedAt = DateTime.UtcNow
                        });
                    }
                }

                // Seed Production Jobs if none exist
                if (!context.ProductionJobs.Any())
                {
                    var jobsConfig = configuration.GetSection(ServerConstants.ConfigInitialProductionJobs)
                        .Get<ProductionJobSeedConfig[]>();

                    if (jobsConfig != null && jobsConfig.Any())
                    {
                        var now = DateTime.UtcNow;
                        foreach (var job in jobsConfig)
                        {
                            context.ProductionJobs.Add(new ProductionJobEntity
                            {
                                JobId = job.JobId,
                                Product = job.Product,
                                Quantity = job.Quantity,
                                Completed = job.Completed,
                                Status = job.Status,
                                DueDate = now.AddDays(job.DueDaysOffset),
                                CreatedAt = now.AddDays(job.CreatedDaysOffset),
                                UpdatedAt = now
                            });
                        }
                    }
                }

                context.SaveChanges();
            }
        }
    }
}
