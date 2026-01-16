using Prism.Ioc;
using Prism.Modularity;
using CyberPrism.Modules.FactoryVisual.Views;
using Prism.Navigation.Regions;
using CyberPrism.Core.Models;

namespace CyberPrism.Modules.FactoryVisual
{
    public class FactoryVisualModule : IModule
    {
        private readonly IRegionManager _regionManager;

        public FactoryVisualModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RegisterViewWithRegion(RegionNames.FactoryVisualRegion, typeof(FactoryVisualView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<FactoryVisualView>();
        }
    }
}

