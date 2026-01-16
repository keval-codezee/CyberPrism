using Xunit;
using global::CyberPrism.Modules.Analytics.ViewModels;
using global::CyberPrism.Tests.Mocks;

namespace CyberPrism.Modules.Analytics.Tests.ViewModels
{
    public class AnalyticsViewModelTests
    {
        [UIFact]
        public void Constructor_ShouldInitializeSeries()
        {
            // Arrange
            var eventAggregator = new Prism.Events.EventAggregator();
            var viewModel = new AnalyticsViewModel(new MockRestService(), eventAggregator);

            // Act & Assert
            Assert.NotNull(viewModel.OeeSeries);
            Assert.Equal(3, viewModel.OeeSeries.Count); // Availability, Performance, Quality
            
            Assert.NotNull(viewModel.DowntimeSeries);
            Assert.Single(viewModel.DowntimeSeries); // Downtime (min)
            
            Assert.NotNull(viewModel.DowntimeLabels);
            Assert.Equal(5, viewModel.DowntimeLabels.Length);
        }
    }
}

