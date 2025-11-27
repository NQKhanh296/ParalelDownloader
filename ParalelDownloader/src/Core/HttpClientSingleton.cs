namespace ParalelDownloader.src.Core
{
    /// <summary>
    /// Provides a thread-safe, globally accessible singleton instance of HttpClient.
    /// </summary>

    public sealed class HttpClientSingleton
    {
        private static readonly HttpClient _instance = CreateClient();

        public static HttpClient Instance => _instance;

        private HttpClientSingleton() { }

        private static HttpClient CreateClient()
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
                "AppleWebKit/537.36 (KHTML, like Gecko) " +
                "Chrome/120.0.0.0 Safari/537.36"
            );

            return client;
        }

    }
}
