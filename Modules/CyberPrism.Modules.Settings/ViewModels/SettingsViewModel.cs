using CyberPrism.Core.Services;
using CyberPrism.Core.Models;
using Prism.Mvvm;
using Prism.Commands;
using System.Threading.Tasks;

using CyberPrism.Modules.Settings.Services;

namespace CyberPrism.Modules.Settings.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        private readonly IAppSettingsService _settingsService;
        private readonly ISuccessDialogService _dialogService;

        public AppSettings Settings => _settingsService.Settings;
        private AppSettings _originalSettings;

        public DelegateCommand SyncCommand { get; private set; }

        public SettingsViewModel(IAppSettingsService settingsService, ISuccessDialogService dialogService)
        {
            _settingsService = settingsService;
            _dialogService = dialogService;

            // Initialize command with a more robust CanExecute check
            SyncCommand = new DelegateCommand(async () => await ExecuteSaveAsync(), CanSave);

            // Initialize original settings for change tracking
            _originalSettings = Settings.Clone();

            // Listen for any property change on the Settings object to enable/disable Save button
            Settings.PropertyChanged += (s, e) => 
            {
                // Must be on UI thread for some Prism versions to reliably update button state
                System.Windows.Application.Current.Dispatcher.Invoke(() => 
                {
                    SyncCommand.RaiseCanExecuteChanged();
                });
            };
        }

        public SettingsViewModel(IAppSettingsService settingsService) : this(settingsService, new SuccessDialogService()) { }

        private bool CanSave()
        {
            // Enable button if current settings differ from the last saved state
            return _originalSettings != null && !Settings.Equals(_originalSettings);
        }

        private async Task ExecuteSaveAsync()
        {
            try
            {
                // 1. First, save locally to ensure no data loss (this is near-instant)
                _settingsService.Save();

                // 2. SHOW SUCCESS DIALOG IMMEDIATELY as requested by user
                System.Windows.Application.Current.Dispatcher.Invoke(() => 
                {
                    _dialogService.ShowSuccessDialog();
                });

                // 3. Attempt to sync with the central server in the background
                await _settingsService.SyncWithServerAsync();
                
                // 4. Update the 'original' snapshot to match the newly saved state
                _originalSettings = Settings.Clone();
                SyncCommand.RaiseCanExecuteChanged();

                // 5. Notify UI that the object might have been updated from the server
                RaisePropertyChanged(nameof(Settings)); 
            }
            catch (System.Exception ex)
            {
                // Log or silently handle server sync errors if the dialog was already shown
                System.Diagnostics.Debug.WriteLine($"Local save OK, but server sync failed: {ex.Message}");
                
                // Update original settings anyway because local save was successful
                _originalSettings = Settings.Clone();
                SyncCommand.RaiseCanExecuteChanged();
            }
        }
    }
}
