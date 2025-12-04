using System.Net;

namespace ParalelDownloader.src.Core
{
    /// <summary>
    /// Provides a globally accessible, thread-safe singleton instance of HttpClient
    /// with predefined headers suitable for downloading files.
    /// </summary>

    public sealed class HttpClientSingleton
    {
        private static readonly HttpClient _instance = CreateClient();

        private HttpClientSingleton() { }

        /// <summary>
        /// Gets the shared HttpClient instance used across the entire application.
        /// </summary>
        public static HttpClient Instance => _instance;

        /// <summary>
        /// Creates and configures the internal HttpClient instance,
        /// including setting a browser-like User-Agent header.
        /// </summary>
        /// <returns>A preconfigured HttpClient instance.</returns>
        private static HttpClient CreateClient()
        {
            var client = new HttpClient();
            client.DefaultRequestVersion = HttpVersion.Version20;
            client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower;

            client.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
                "AppleWebKit/537.36 (KHTML, like Gecko) " +
                "Chrome/120.0.0.0 Safari/537.36"
            );

            return client;
        }

    }
}
