using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Prism.Events;
using CyberPrism.Core.Constants;

namespace CyberPrism.Core.Services
{
    /// <summary>
    /// Implementation of IRestService that uses HttpClient to communicate with the backend.
    /// </summary>
    public class RestService : IRestService
    {
        private readonly HttpClient _httpClient;
        private readonly IEventAggregator _eventAggregator;
        private bool? _lastConnectionStatus = null;

        /// <summary>
        /// Gets current connection status. Returns true if unknown.
        /// </summary>
        public bool IsConnected => _lastConnectionStatus ?? true;

        /// <summary>
        /// Initializes a new instance of the RestService with a shared EventAggregator.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator for publishing status changes.</param>
        public RestService(IEventAggregator eventAggregator)
        {
            // Configure HttpClient with base address and timeout
            _httpClient = new HttpClient { BaseAddress = new Uri(AppConstants.BaseApiUrl), Timeout = TimeSpan.FromSeconds(5) };
            _eventAggregator = eventAggregator;
        }

        /// <summary>
        /// Explicitly checks the connection status by hitting the dashboard endpoint.
        /// </summary>
        /// <returns>True if the server is reachable and returns success.</returns>
        public async Task<bool> CheckConnectionAsync()
        {
            try
            {
                // Hit a safe endpoint to verify server availability
                var response = await _httpClient.GetAsync(AppConstants.DashboardEndpoint);
                UpdateStatus(response.IsSuccessStatusCode);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                UpdateStatus(false);
                return false;
            }
        }

        /// <summary>
        /// Internal helper to update connection status and notify subscribers via EventAggregator.
        /// </summary>
        /// <param name="isConnected">The new connection status.</param>
        private void UpdateStatus(bool isConnected)
        {
            if (_lastConnectionStatus != isConnected)
            {
                _lastConnectionStatus = isConnected;
                // Publish event so the UI can respond to connectivity changes
                _eventAggregator.GetEvent<Events.ConnectionStatusEvent>().Publish(isConnected);
            }
        }

        /// <summary>
        /// Fetches data from a specific API endpoint using HTTP GET.
        /// </summary>
        /// <typeparam name="T">The type of object to return.</typeparam>
        /// <param name="endpoint">The API endpoint (e.g., "production").</param>
        /// <returns>Deserialized content or null on failure.</returns>
        public async Task<T?> GetAsync<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                if (response.IsSuccessStatusCode)
                {
                    UpdateStatus(true);
                    // Reads the response body as JSON and deserializes it to the specified type
                    return await response.Content.ReadFromJsonAsync<T>();
                }
                UpdateStatus(false);
                return default;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[RestService] GET Error: {ex.Message}");
                UpdateStatus(false);
                return default;
            }
        }

        /// <summary>
        /// Sends data to a specific API endpoint using HTTP POST as JSON.
        /// </summary>
        /// <typeparam name="T">The type of the data object to send.</typeparam>
        /// <param name="endpoint">The API endpoint.</param>
        /// <param name="data">The data to be serialized to JSON.</param>
        /// <returns>True if the server returned a success status code.</returns>
        public async Task<bool> PostAsync<T>(string endpoint, T data)
        {
            try
            {
                // Serializes data to JSON and sends it as the request body
                var response = await _httpClient.PostAsJsonAsync(endpoint, data);
                bool success = response.IsSuccessStatusCode;
                UpdateStatus(success);
                return success;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[RestService] POST Error: {ex.Message}");
                UpdateStatus(false);
                return false;
            }
        }
    }
}
