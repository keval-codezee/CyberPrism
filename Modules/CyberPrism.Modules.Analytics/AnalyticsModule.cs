using Prism.Ioc;
using Prism.Modularity;

using CyberPrism.Modules.Analytics.Views;

namespace CyberPrism.Modules.Analytics
{
    public class AnalyticsModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<AnalyticsView>();
        }
    }
}

