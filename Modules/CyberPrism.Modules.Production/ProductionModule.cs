using Prism.Ioc;
using Prism.Modularity;

using CyberPrism.Modules.Production.Views;
using CyberPrism.Modules.Production.Services;

namespace CyberPrism.Modules.Production
{
    public class ProductionModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Register Singleton services
            containerRegistry.RegisterSingleton<IProductionDataService, ProductionDataService>();
            
            containerRegistry.RegisterForNavigation<ProductionView>();
        }
    }
}

