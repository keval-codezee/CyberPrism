using Prism.Ioc;
using Prism.Modularity;

using CyberPrism.Modules.Settings.Views;

namespace CyberPrism.Modules.Settings
{
    public class SettingsModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<SettingsView>();
            containerRegistry.Register<Services.ISuccessDialogService, Services.SuccessDialogService>();
        }
    }
}

