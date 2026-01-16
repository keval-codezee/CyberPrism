using Prism.Ioc;
using Prism.Modularity;
using Prism.Navigation.Regions;
using CyberPrism.Modules.Dashboard.Views;
using CyberPrism.Core.Models;

namespace CyberPrism.Modules.Dashboard
{
    public class DashboardModule : IModule
    {
        private readonly IRegionManager _regionManager;

        public DashboardModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            // Initialization logic if needed (navigation is handled by ShellViewModel)
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
			// Register DashboardView with the navigation system
			// Key: Use the view name "DashboardView" as the navigation key
			containerRegistry.RegisterForNavigation<DashboardView>();
        }
    }
}

