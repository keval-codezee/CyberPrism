using Prism.Mvvm; // Kept because BindableBase is used
using Prism.Commands; // Added from Code Edit
using System.Collections.ObjectModel;
using System;
using System.Windows;
using System.Threading.Tasks; // Kept and de-duplicated
using CyberPrism.Core.Models;
using CyberPrism.Modules.Production.Services;

namespace CyberPrism.Modules.Production.ViewModels
{
    public class ProductionViewModel : BindableBase
    {
        private readonly IProductionDataService _dataService;
        private readonly Prism.Events.IEventAggregator _eventAggregator;
        private bool _isConnected = false;
        public bool IsConnected
        {
            get => _isConnected;
            set => SetProperty(ref _isConnected, value);
        }
        private ObservableCollection<ProductionJob> _jobs;

        public ObservableCollection<ProductionJob> Jobs
        {
            get { return _jobs; }
            set { SetProperty(ref _jobs, value); }
        }

        private readonly System.Windows.Threading.DispatcherTimer _refreshTimer;

        public ProductionViewModel(IProductionDataService dataService, Prism.Events.IEventAggregator eventAggregator, Core.Services.IRestService restService)
        {
            _dataService = dataService;
            _eventAggregator = eventAggregator;
            IsConnected = restService.IsConnected;
            _eventAggregator.GetEvent<Core.Events.ConnectionStatusEvent>().Subscribe(status => IsConnected = status);

            // Initialize empty collection
            Jobs = new ObservableCollection<ProductionJob>();
            
            // Configure timer for real-time data polling (every 5 seconds)
            _refreshTimer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            _refreshTimer.Tick += async (s, e) => await LoadProductionJobsAsync();
            _refreshTimer.Start();

            // Initial load
            _ = LoadProductionJobsAsync();
        }

        private async Task LoadProductionJobsAsync()
        {
            var latestJobs = await _dataService.GetProductionJobsAsync();
            if (latestJobs == null) return;

            // Synchronize data on the UI thread (Reconciliation) to prevent flickering
            Application.Current.Dispatcher.Invoke(() =>
            {
                // 1. Update or add jobs
                foreach (var latestJob in latestJobs)
                {
                    var existingJob = System.Linq.Enumerable.FirstOrDefault(Jobs, j => j.JobId == latestJob.JobId);
                    if (existingJob != null)
                    {
                        // Update existing job properties
                        existingJob.Product = latestJob.Product;
                        existingJob.Quantity = latestJob.Quantity;
                        existingJob.Completed = latestJob.Completed;
                        existingJob.Status = latestJob.Status;
                        existingJob.DueDate = latestJob.DueDate;
                    }
                    else
                    {
                        // Add new job
                        Jobs.Add(latestJob);
                    }
                }

                // 2. Remove jobs that no longer exist on the server
                for (int i = Jobs.Count - 1; i >= 0; i--)
                {
                    var currentJobId = Jobs[i].JobId;
                    if (!System.Linq.Enumerable.Any(latestJobs, j => j.JobId == currentJobId))
                    {
                        Jobs.RemoveAt(i);
                    }
                }
            });
        }
    }
}
