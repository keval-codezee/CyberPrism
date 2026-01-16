using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CyberPrism.Core.Models;
using CyberPrism.Core.Services;

namespace CyberPrism.Modules.Production.Services
{
    public class ProductionDataService : IProductionDataService
    {
        private readonly IRestService _restService;

        public ProductionDataService(IRestService restService)
        {
            _restService = restService;
        }

        public async Task<ObservableCollection<ProductionJob>> GetProductionJobsAsync()
        {
            var jobs = await _restService.GetAsync<List<ProductionJob>>("production");
            return jobs != null ? new ObservableCollection<ProductionJob>(jobs) : new ObservableCollection<ProductionJob>();
        }
    }
}

