namespace CyberPrism.Core.Services
{
    /// <summary>
    /// Interface for the REST communication service used to interact with the backend API.
    /// </summary>
	public interface IRestService
	{
        /// <summary>
        /// Gets a value indicating whether the service is currently connected to the server.
        /// </summary>
		bool IsConnected { get; }

        /// <summary>
        /// Explicitly checks the connection status by hitting a lightweight endpoint.
        /// </summary>
        /// <returns>True if connection is successful, false otherwise.</returns>
		Task<bool> CheckConnectionAsync();

        /// <summary>
        /// Performs an asynchronous GET request to the specified endpoint.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the JSON response into.</typeparam>
        /// <param name="endpoint">The relative API endpoint path.</param>
        /// <returns>The deserialized object of type T, or default if the request fails.</returns>
		Task<T?> GetAsync<T>(string endpoint);

        /// <summary>
        /// Performs an asynchronous POST request to the specified endpoint with the provided data.
        /// </summary>
        /// <typeparam name="T">The type of the data to be sent.</typeparam>
        /// <param name="endpoint">The relative API endpoint path.</param>
        /// <param name="data">The data object to be sent as JSON in the request body.</param>
        /// <returns>True if the request was successful, false otherwise.</returns>
		Task<bool> PostAsync<T>(string endpoint, T data);
	}
}
