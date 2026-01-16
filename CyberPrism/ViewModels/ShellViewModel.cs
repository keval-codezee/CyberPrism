using CyberPrism.Core.Services;
using CyberPrism.Core.Models;
using CyberPrism.Core.Constants;

namespace CyberPrism.ViewModels
{
    public class ShellViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private readonly IAppSettingsService _settingsService;
        private readonly IModuleManager _moduleManager;
        private readonly IRestService _restService;

        public AppSettings Settings => _settingsService.Settings;

        private bool _isBusy = true;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public DelegateCommand<string> NavigateCommand { get; private set; }

        public ShellViewModel(IRegionManager regionManager, 
                              IAppSettingsService settingsService, 
                              IModuleManager moduleManager,
                              IRestService restService)
        {
            _regionManager = regionManager;
            _settingsService = settingsService;
            _moduleManager = moduleManager;
            _restService = restService;
            NavigateCommand = new DelegateCommand<string>(Navigate);

            // Initialize connection and module loading
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            IsBusy = true;
            bool firstAttempt = true;

            while (true)
            {
                // 1. Try to connect
                bool connected = await _restService.CheckConnectionAsync();

                if (connected)
                {
                    // 1. Sync with server to get the latest data/state
                    await _settingsService.SyncWithServerAsync();
                    
                    // Notify UI to refresh visibility based on NEWLY synced settings
                    RaisePropertyChanged(nameof(Settings));

                    // 2. Navigation
                    // Since modules were already loaded (or not) in Bootstrapper, 
                    // we just need to navigate to the enabled view.
                    if (Settings.ShowDashboard)
                        Navigate(AppConstants.DashboardView);
                    else
                        Navigate(AppConstants.SettingsView);

                    break; // Exit the reconnection loop
                }
                else
                {
                    // 4. Connection failed
                    if (firstAttempt)
                    {
                        Navigate(AppConstants.ErrorView);
                        firstAttempt = false;
                    }
                    
                    // Wait 5 seconds before retrying
                    await Task.Delay(5000);
                }
            }

            IsBusy = false;
        }

        private void Navigate(string navigatePath)
        {
            if (navigatePath != null)
                _regionManager.RequestNavigate(RegionNames.ContentRegion, navigatePath);
        }
    }
}

