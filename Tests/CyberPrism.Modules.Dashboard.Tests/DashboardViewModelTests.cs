using Xunit;
using CyberPrism.Modules.Dashboard.ViewModels;
using System.Linq;
using CyberPrism.Tests.Mocks;

namespace CyberPrism.Modules.Dashboard.Tests.ViewModels
{
    public class DashboardViewModelTests
    {
        [UIFact]
        public void Title_ShouldBeDashboard()
        {
            // Arrange
            var viewModel = new DashboardViewModel(new Prism.Events.EventAggregator(), new MockRestService());

            // Act
            var title = viewModel.Title;

            // Assert
            Assert.Equal("Dashboard", title);
        }

        [UIFact]
        public void Constructor_ShouldInitializeSeriesData()
        {
            // Arrange
            var viewModel = new DashboardViewModel(new Prism.Events.EventAggregator(), new MockRestService());

            // Act
            var monthlySummary = viewModel.MonthlySummarySeries;
            var performance = viewModel.PerformanceSeries;
            var quality = viewModel.QualitySeries;
            var power = viewModel.PowerConsumptionSeries;

            // Assert
            Assert.NotNull(monthlySummary);
            Assert.NotEmpty(monthlySummary);
            Assert.Equal(2, monthlySummary.Count); // Production and Target

            Assert.NotNull(performance);
            Assert.NotEmpty(performance);

            Assert.NotNull(quality);
            Assert.Equal(2, quality.Count); // Pass and Fail

            Assert.NotNull(power);
            Assert.Single(power);
        }
    }
}

