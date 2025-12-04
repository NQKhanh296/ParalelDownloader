namespace ParalelDownloader.src.Core
{
    /// <summary>
    /// Maintains the count of successfully downloaded files.
    /// Uses thread-safe Interlocked.Increment since multiple workers
    /// may update the counter concurrently.
    /// </summary>
    public static class DownloadStatistics
    {
        private static int _successCount;

        /// <summary>
        /// Resets the success counter before a new download batch starts.
        /// </summary>
        public static void Reset() => _successCount = 0;

        /// <summary>
        /// Increments the success counter. Called by workers after a successful download.
        /// </summary>
        public static void IncrementSuccess()
        {
            Interlocked.Increment(ref _successCount);
        }

        /// <summary>
        /// Returns the total number of successfully downloaded files.
        /// </summary>
        public static int SuccessCount => _successCount;
    }
}
