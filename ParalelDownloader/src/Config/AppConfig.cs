namespace ParalelDownloader.src.Config
{
    public class AppConfig
    {
        /// <summary>
        /// AppConfig vypočítává optimální počet workerů pro thread pool.
        /// Počet workerů = minimum mezi počtem URL a počtem CPU jader.
        /// </summary>
        public int CalculateOptimalWorkers(int urlCount)
        {
            int cpuCores = Environment.ProcessorCount;
            return Math.Min(cpuCores, urlCount);
        }
    }
}
