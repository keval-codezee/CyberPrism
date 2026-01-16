using CyberPrism.Modules.Production.Tests.Mocks;
using CyberPrism.Modules.Production.ViewModels;

namespace CyberPrism.Modules.Production.Tests.ViewModels
{
    public class ProductionViewModelTests
    {
        [Fact]
        public void Constructor_ShouldInitializeJobs()
        {
            // Arrange
            var mockService = new MockProductionDataService();
            var eventAggregator = new Prism.Events.EventAggregator();
            var mockRest = new CyberPrism.Modules.Production.Tests.Mocks.MockRestService();
            var viewModel = new ProductionViewModel(mockService, eventAggregator, mockRest);

            // Act & Assert
            Assert.NotNull(viewModel.Jobs);
            // Note: Since data is loaded asynchronously, the instantaneous state after the constructor might be empty.
            // However, in a simple constructor test, we at least verify it's initialized.
        }
    }
}
