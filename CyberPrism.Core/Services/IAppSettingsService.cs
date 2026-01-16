using CyberPrism.Core.Models;

namespace CyberPrism.Core.Services
{
    public interface IAppSettingsService
    {
        AppSettings Settings { get; }
        void Save();
        Task SyncWithServerAsync();
    }
}

