using LiveCharts;
using LiveCharts.Wpf;
using Prism.Mvvm;
using System;
using System.Windows;
using System.Windows.Threading;
using System.Threading.Tasks;
using Prism.Events;
using System.Windows.Media;
using CyberPrism.Core.Constants;

namespace CyberPrism.Modules.Dashboard.ViewModels
{
    /// <summary>
    /// Dashboard ViewModel - Manages key metrics and overview charts.
    /// </summary>
    public class DashboardViewModel : BindableBase
    {
        private bool _isConnected = false;
        public bool IsConnected
        {
            get => _isConnected;
            set => SetProperty(ref _isConnected, value);
        }

        private string _title = AppConstants.DashboardTitle;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private SeriesCollection _monthlySummarySeries;
        private SeriesCollection _performanceSeries;
        private SeriesCollection _qualitySeries;
        private SeriesCollection _powerConsumptionSeries;
        private string[] _labels;

        public SeriesCollection MonthlySummarySeries
        {
            get { return _monthlySummarySeries; }
            set { SetProperty(ref _monthlySummarySeries, value); }
        }

        public SeriesCollection PerformanceSeries
        {
            get { return _performanceSeries; }
            set { SetProperty(ref _performanceSeries, value); }
        }

        public SeriesCollection QualitySeries
        {
            get { return _qualitySeries; }
            set { SetProperty(ref _qualitySeries, value); }
        }

        public SeriesCollection PowerConsumptionSeries
        {
            get { return _powerConsumptionSeries; }
            set { SetProperty(ref _powerConsumptionSeries, value); }
        }

        public string[] Labels
        {
            get { return _labels; }
            set { SetProperty(ref _labels, value); }
        }

        private string _systemStatus = AppConstants.SystemOperational;
        public string SystemStatus
        {
            get { return _systemStatus; }
            set { SetProperty(ref _systemStatus, value); }
        }

        private Brush _systemStatusColor = Brushes.LightGreen;
        public Brush SystemStatusColor
        {
            get { return _systemStatusColor; }
            set { SetProperty(ref _systemStatusColor, value); }
        }
        public Func<double, string> YFormatter { get; set; }

        private readonly global::CyberPrism.Core.Services.IRestService _restService;
        private readonly DispatcherTimer _refreshTimer;
        private bool _isLoading;

        private string _efficiencyText = "0.00%";
        public string EfficiencyText
        {
            get => _efficiencyText;
            set => SetProperty(ref _efficiencyText, value);
        }

        private string _unitsProducedText = "0";
        public string UnitsProducedText
        {
            get => _unitsProducedText;
            set => SetProperty(ref _unitsProducedText, value);
        }

        // Keep references to ChartValues to update data without redrawing diagrams
        private readonly ChartValues<double> _productionValues = new ChartValues<double>();
        private readonly ChartValues<double> _targetValues = new ChartValues<double>();
        private readonly ChartValues<double> _efficiencyValues = new ChartValues<double>();
        private readonly ChartValues<double> _qualityPassValues = new ChartValues<double> { 0 };
        private readonly ChartValues<double> _qualityFailValues = new ChartValues<double> { 0 };
        private readonly ChartValues<double> _powerValues = new ChartValues<double>();

        /// <summary>
        /// Initializes a new instance of the DashboardViewModel class and starts data refresh.
        /// </summary>
        public DashboardViewModel(IEventAggregator eventAggregator, Core.Services.IRestService restService)
        {
            _restService = restService;
            IsConnected = _restService.IsConnected;
            eventAggregator.GetEvent<Core.Events.DowntimeAlertEvent>().Subscribe(OnDowntimeAlert);
            eventAggregator.GetEvent<Core.Events.ConnectionStatusEvent>().Subscribe(status => IsConnected = status);

            MonthlySummarySeries = new SeriesCollection
            {
                new LineSeries { Title = AppConstants.ProductionTitle, Values = _productionValues, PointGeometrySize = 10, LineSmoothness = 0 },
                new LineSeries { Title = AppConstants.TargetTitle, Values = _targetValues, PointGeometry = null, Fill = Brushes.Transparent }
            };

            PerformanceSeries = new SeriesCollection
            {
                new LineSeries { Title = AppConstants.EfficiencyTitle, Values = _efficiencyValues, PointGeometry = null, AreaLimit = 0 }
            };

            QualitySeries = new SeriesCollection
            {
                new PieSeries { Title = AppConstants.PassTitle, Values = _qualityPassValues, DataLabels = true },
                new PieSeries { Title = AppConstants.FailTitle, Values = _qualityFailValues, DataLabels = true }
            };

            PowerConsumptionSeries = new SeriesCollection
            {
                new ColumnSeries { Title = AppConstants.PowerTitle, Values = _powerValues }
            };

            Labels = new[] { AppConstants.MonthJan, AppConstants.MonthFeb, AppConstants.MonthMar, AppConstants.MonthApr, AppConstants.MonthMay };
            YFormatter = value => value.ToString("C");

            _refreshTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            _refreshTimer.Tick += async (s, e) => await LoadDataAsync();
            _refreshTimer.Start();

            _ = LoadDataAsync();
        }

        /// <summary>
        /// Asynchronously fetches dashboard data from the REST service and updates the UI.
        /// </summary>
        private async Task LoadDataAsync()
        {
            if (_isLoading) return;
            _isLoading = true;

            try
            {
                var dataSet = await _restService.GetAsync<Core.Models.DashboardData>(AppConstants.DashboardEndpoint);
                if (dataSet == null) return;

                // Smoothly update data on the UI thread without redrawing the entire chart
                Application.Current.Dispatcher.Invoke(() =>
                {
                    UpdateChartValues(_productionValues, dataSet.ProductionValues);
                    UpdateChartValues(_targetValues, dataSet.TargetValues);
                    UpdateChartValues(_efficiencyValues, dataSet.EfficiencyValues);
                    UpdateChartValues(_powerValues, dataSet.PowerConsumptionValues);

                    if (_qualityPassValues.Count > 0) _qualityPassValues[0] = dataSet.QualityPass;
                    if (_qualityFailValues.Count > 0) _qualityFailValues[0] = dataSet.QualityFail;

                    // Update metrics
                    var total = dataSet.QualityPass + dataSet.QualityFail;
                    var efficiency = total > 0 ? (dataSet.QualityPass / total * 100) : 0;
                    EfficiencyText = $"{efficiency:N2}%";
                    UnitsProducedText = $"{total:N0}";
                });
            }
            finally
            {
                _isLoading = false;
            }
        }

        /// <summary>
        /// Updates an existing ChartValues collection with new data to avoid redraw flicker.
        /// </summary>
        private void UpdateChartValues(ChartValues<double> current, System.Collections.Generic.List<double> latest)
        {
            if (latest == null) return;
            
            // Avoid using Clear() to prevent flickering
            while (current.Count < latest.Count) current.Add(0);
            while (current.Count > latest.Count) current.RemoveAt(current.Count - 1);

            for (int i = 0; i < latest.Count; i++)
            {
                if (Math.Abs(current[i] - latest[i]) > 0.001)
                {
                    current[i] = latest[i];
                }
            }
        }

        /// <summary>
        /// Event handler for downtime alerts received via the EventAggregator.
        /// </summary>
        private void OnDowntimeAlert(Core.Events.DowntimeAlertMessage message)
        {
            // Change system status on the dashboard
            SystemStatus = $"ALERT: {message.Reason} ({message.Duration:N0} min)";
            SystemStatusColor = Brushes.OrangeRed;

            // Revert after 5 seconds
            Task.Delay(5000).ContinueWith(t => 
            {
                SystemStatus = AppConstants.SystemOperational;
                SystemStatusColor = Brushes.LightGreen;
            });
        }
    }
}
