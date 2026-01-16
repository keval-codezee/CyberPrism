using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CyberPrism.Core.Models;
using CyberPrism.Modules.Production.Services;

namespace CyberPrism.Modules.Production.Tests.Mocks
{
    public class MockProductionDataService : IProductionDataService
    {
        public Task<ObservableCollection<ProductionJob>> GetProductionJobsAsync()
        {
            return Task.FromResult(new ObservableCollection<ProductionJob>
            {
                new ProductionJob { JobId = "JB-TEST", Product = "Test Product", Quantity = 100, Completed = 50, Status = "Running", DueDate = DateTime.Now }
            });
        }
    }
}

