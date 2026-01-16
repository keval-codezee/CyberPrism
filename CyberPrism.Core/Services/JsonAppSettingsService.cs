using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using CyberPrism.Core.Models;
using CyberPrism.Core.Services;
using CyberPrism.Core.Constants;

namespace CyberPrism.Core.Services
{
    public class JsonAppSettingsService : IAppSettingsService
    {
        private readonly string _filePath;
        private readonly IRestService? _restService; // Made nullable

        public AppSettings Settings { get; } = new AppSettings(); // Initialize with default, now only gettable

        public JsonAppSettingsService(IRestService? restService) // Made parameter nullable
        {
            _restService = restService; // Assigned injected service (can be null)
            var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppConstants.AppDataFolderName);
            Directory.CreateDirectory(folder);
            _filePath = Path.Combine(folder, AppConstants.SettingsFileName);

            Load();
        }

        public async Task SyncWithServerAsync()
        {
            if (_restService == null) return;

            try
            {
                // 1. Try to push local settings to server
                await _restService.PostAsync(AppConstants.SettingsEndpoint, Settings);

                // 2. Pull latest from server to ensure sync
                var remoteSettings = await _restService.GetAsync<AppSettings>(AppConstants.SettingsEndpoint);
                if (remoteSettings != null)
                {
                    // Copy properties instead of replacing object to keep event listeners alive
                    UpdateSettings(remoteSettings);
                    Save(); // Update local cache
                }
            }
            catch (Exception)
            {
                // If server push fails, we still allow local saving
                Save();
            }
        }

        private void UpdateSettings(AppSettings source)
        {
            Settings.ShowDashboard = source.ShowDashboard;
            Settings.ShowProduction = source.ShowProduction;
            Settings.ShowAnalytics = source.ShowAnalytics;
            Settings.ShowFactoryVisual = source.ShowFactoryVisual;
            Settings.MachineName = source.MachineName;
            Settings.IpAddress = source.IpAddress;
            Settings.IsDarkMode = source.IsDarkMode;
            Settings.NotificationsEnabled = source.NotificationsEnabled;
        }

        private void Load()
        {
            if (File.Exists(_filePath))
            {
                try
                {
                    var json = File.ReadAllText(_filePath);
                    var loaded = JsonSerializer.Deserialize<AppSettings>(json);
                    if (loaded != null)
                    {
                        UpdateSettings(loaded);
                    }
                }
                catch
                {
                    // Defaults are already applied
                }
            }
        }

        public void Save()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(Settings, options);
            File.WriteAllText(_filePath, json);
        }
    }
}

