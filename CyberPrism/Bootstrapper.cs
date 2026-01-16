using System.Windows;
using CyberPrism.Core.Services;
using CyberPrism.Core.Views;
using CyberPrism.Modules.Analytics;
using CyberPrism.Modules.Dashboard;
using CyberPrism.Modules.FactoryVisual;
using CyberPrism.Modules.Production;
using CyberPrism.Modules.Settings;
using CyberPrism.ViewModels;
using CyberPrism.Views;
using CyberPrism.Core.Models;
using System.IO;
using System.Text.Json;
using System;
using Prism.Ioc;
using Prism.Modularity;
using CyberPrism.Core.Constants;

namespace CyberPrism
{
    public class Bootstrapper : PrismBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            // Resolve and create the main shell from the DI container
            return Container.Resolve<Shell>();
        }

        protected override void InitializeShell(DependencyObject shell)
        {
            Application.Current.MainWindow = (Window)shell;
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            // 1. Get settings path (same as JsonAppSettingsService)
            var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppConstants.AppDataFolderName);
            var filePath = Path.Combine(folder, AppConstants.SettingsFileName);
            AppSettings settings = new AppSettings();

            // 2. Load settings synchronously to decide which modules to add to catalog
            if (File.Exists(filePath))
            {
                try
                {
                    var json = File.ReadAllText(filePath);
                    settings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
                }
                catch { /* Fallback to defaults */ }
            }

            // 3. Conditionally add modules - No OnDemand needed, decisions happen here
            if (settings.ShowDashboard)
                moduleCatalog.AddModule<DashboardModule>();

            if (settings.ShowProduction)
                moduleCatalog.AddModule<ProductionModule>();

            if (settings.ShowAnalytics)
                moduleCatalog.AddModule<AnalyticsModule>();

            // The following modules are always essential for the UI structure
            moduleCatalog.AddModule<FactoryVisualModule>();
            moduleCatalog.AddModule<SettingsModule>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Register as Singleton: Entire application shares one SettingsService instance
            containerRegistry.RegisterSingleton<IAppSettingsService, JsonAppSettingsService>();
            
            // Register RestService as singleton for API communication
            containerRegistry.RegisterSingleton<IRestService, RestService>();

            // Register ConnectionErrorView from Core for navigation mapping
            containerRegistry.RegisterForNavigation<ConnectionErrorView>("ErrorView");
        }
    }
}
