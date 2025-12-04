namespace ParalelDownloader.src.Config
{
    /// <summary>
    /// Provides configuration logic for determining the optimal number of workers
    /// based on system hardware and workload size.
    /// </summary>
    public class AppConfig
    {
        /// <summary>
        /// Calculates the optimal number of worker threads.
        /// The result is the minimum of the CPU core count and the number of URLs.
        /// </summary>
        /// <param name="urlCount">Number of URLs to download.</param>
        /// <returns>The recommended number of worker threads.</returns>
        public int CalculateOptimalWorkers(int urlCount)
        {
            int cpuCores = Environment.ProcessorCount;
            return Math.Min(cpuCores, urlCount);
        }
    }
}
