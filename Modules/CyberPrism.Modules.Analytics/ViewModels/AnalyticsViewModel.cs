using LiveCharts;
using LiveCharts.Wpf;
using Prism.Mvvm;
using Prism.Events;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows;
using CyberPrism.Core.Models;
using CyberPrism.Core.Constants;

namespace CyberPrism.Modules.Analytics.ViewModels
{
    /// <summary>
    /// Analytics ViewModel - Implements the producer-consumer pattern to handle real-time sensor data and update charts.
    /// </summary>
    public class AnalyticsViewModel : BindableBase, IDisposable
    {
        // ===== Producer-Consumer =====
        private readonly ConcurrentQueue<SensorDataPoint> _dataQueue;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private Task _producerTask;
        private Task _consumerTask;
        private readonly IEventAggregator _eventAggregator;
		private int dataPointCount = 0;

		// ===== UI State Variables =====
		private SeriesCollection _oeeSeries;
        public SeriesCollection OeeSeries  {           
      
            get { return _oeeSeries; }
            set { SetProperty(ref _oeeSeries, value); }
        }

        private SeriesCollection _downtimeSeries;
        public SeriesCollection DowntimeSeries
        {
            get { return _downtimeSeries; }
            set { SetProperty(ref _downtimeSeries, value); }
        }

        public string[] DowntimeLabels { get; set; }
        public Func<double, string> Formatter { get; set; }

        private readonly Core.Services.IRestService _restService;

        /// <summary>
        /// Initializes a new instance of the AnalyticsViewModel class.
        /// </summary>
        public AnalyticsViewModel(Core.Services.IRestService restService, IEventAggregator eventAggregator)
        {
            _restService = restService;
            _eventAggregator = eventAggregator;
            IsConnected = _restService.IsConnected;
            eventAggregator.GetEvent<Core.Events.ConnectionStatusEvent>().Subscribe(status => IsConnected = status);
            
            _dataQueue = new ConcurrentQueue<SensorDataPoint>();
            _cancellationTokenSource = new CancellationTokenSource();

            InitializeCharts();
            StartProducerConsumer();
        }

        private bool _isConnected = false;
        public bool IsConnected
        {
            get => _isConnected;
            set => SetProperty(ref _isConnected, value);
        }

        private string _title = AppConstants.AnalyticsTitle;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        /// <summary>
        /// Configures initial chart series and axis settings.
        /// </summary>
        private void InitializeCharts()
        {
            OeeSeries = new SeriesCollection
            {
                new StackedColumnSeries
                {
                    Values = new ChartValues<double> { 0, 0, 0, 0, 0, 0, 0 },
                    StackMode = StackMode.Values,
                    DataLabels = false,
                    Title = AppConstants.AvailabilityTitle
                },
                new StackedColumnSeries
                {
                    Values = new ChartValues<double> { 0, 0, 0, 0, 0, 0, 0 },
                    StackMode = StackMode.Values,
                    DataLabels = false,
                    Title = AppConstants.PerformanceTitle
                },
                new StackedColumnSeries
                {
                    Values = new ChartValues<double> { 0, 0, 0, 0, 0, 0, 0 },
                    StackMode = StackMode.Values,
                    DataLabels = false,
                    Title = AppConstants.QualityTitle
                }
            };

            DowntimeSeries = new SeriesCollection
            {
                new RowSeries
                {
                    Title = AppConstants.DowntimeChartTitle,
                    Values = new ChartValues<double> { 0, 0, 0, 0, 0 },
                    DataLabels = true,
                    Foreground = System.Windows.Media.Brushes.White
                }
            };

            DowntimeLabels = new[] { 
                AppConstants.DowntimeMechanical, 
                AppConstants.DowntimeElectrical, 
                AppConstants.DowntimeMaterial, 
                AppConstants.DowntimeOperator, 
                AppConstants.DowntimeOther 
            };
            Formatter = value => value.ToString("N0") + AppConstants.UnitMinutes;
        }

        /// <summary>
        /// Starts background tasks for fetching (Producer) and processing (Consumer) sensor data.
        /// </summary>
        private void StartProducerConsumer()
        {
            // Producer: Pulls real-time sensor data from server
            _producerTask = Task.Run(async () =>
			{
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    await Task.Delay(500, _cancellationTokenSource.Token);

                    var dataPoints = await _restService.GetAsync<List<SensorDataPoint>>(AppConstants.AnalyticsEndpoint);
                    if (dataPoints != null)
                    {
                        foreach (var dp in dataPoints)
                        {
                            _dataQueue.Enqueue(dp);
                        }
                        dataPointCount++;
                        System.Diagnostics.Debug.WriteLine($"[Producer] Fetched batch {dataPointCount}, current queue size: {_dataQueue.Count}");
                    }
                }
            }, _cancellationTokenSource.Token);

            // Consumer: Processes data and updates UI on a background thread
            _consumerTask = Task.Run(async () =>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    if (_dataQueue.TryDequeue(out var dataPoint))
                    {
                        await ProcessDataPointAsync(dataPoint);
                    }
                    else
                    {
                        await Task.Delay(100, _cancellationTokenSource.Token);
                    }
                }
            }, _cancellationTokenSource.Token);
        }

        /// <summary>
        /// Processes a single sensor data point and updates the UI accordingly.
        /// </summary>
        private async Task ProcessDataPointAsync(SensorDataPoint dataPoint)
        {
            // Simulate processing delay
            await Task.Delay(10);

			// Update chart on the UI thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (dataPoint.DataType == AppConstants.DataTypeOee)
                {
                    // Server returns SensorId in format: "Sensor-X"
                    if (int.TryParse(dataPoint.SensorId.Replace(AppConstants.SensorPrefix, ""), out int sensorIndex))
                    {
                        var dayIndex = sensorIndex % 7; 
                        var componentIndex = sensorIndex % 3; // 0=Availability, 1=Performance, 2=Quality

                        if (OeeSeries[componentIndex].Values.Count > dayIndex)
                        {
                            OeeSeries[componentIndex].Values[dayIndex] = dataPoint.Value;
                        }
                    }
                }
                else if (dataPoint.DataType == AppConstants.DataTypeDowntime)
                {
                    if (int.TryParse(dataPoint.SensorId.Replace(AppConstants.SensorPrefix, ""), out int reasonIndex))
                    {
                        reasonIndex = reasonIndex % 5; // Map to 5 downtime reasons

                        if (DowntimeSeries[0].Values.Count > reasonIndex)
                        {
                            DowntimeSeries[0].Values[reasonIndex] = dataPoint.Value;

                            // Alert through EventAggregator if downtime exceeds threshold
                            if (dataPoint.Value > 100)
                            {
                                _eventAggregator.GetEvent<Core.Events.DowntimeAlertEvent>().Publish(new Core.Events.DowntimeAlertMessage
                                {
                                    Reason = DowntimeLabels[reasonIndex],
                                    Duration = dataPoint.Value,
                                    Severity = AppConstants.SeverityHigh
                                });
                            }
                        }
                    }
                }
            });

            System.Diagnostics.Debug.WriteLine($"[Consumer] Processed data point: {dataPoint.SensorId} = {dataPoint.Value}");
        }

        /// <summary>
        /// Releases all resources used by the AnalyticsViewModel.
        /// </summary>
        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();

            try
            {
                Task.WaitAll(new[] { _producerTask, _consumerTask }, TimeSpan.FromSeconds(2));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception during disposal: {ex.Message}");
            }

            _cancellationTokenSource?.Dispose(); 
		}
    }
}
