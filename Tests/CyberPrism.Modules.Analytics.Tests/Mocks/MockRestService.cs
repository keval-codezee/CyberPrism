using global::CyberPrism.Core.Services;
using System.Threading.Tasks;

namespace CyberPrism.Tests.Mocks
{
    public class MockRestService : IRestService
    {
        public bool IsConnected => true;

        public Task<bool> CheckConnectionAsync()
        {
            return Task.FromResult(true);
        }

        public Task<T?> GetAsync<T>(string endpoint)
        {
            return Task.FromResult(default(T));
        }

        public Task<bool> PostAsync<T>(string endpoint, T data)
        {
            return Task.FromResult(true);
        }
    }
}

