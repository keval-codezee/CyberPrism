using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CyberPrism.Core.Models;

namespace CyberPrism.Modules.Production.Services
{
    public interface IProductionDataService
    {
        Task<ObservableCollection<ProductionJob>> GetProductionJobsAsync();
    }
}

