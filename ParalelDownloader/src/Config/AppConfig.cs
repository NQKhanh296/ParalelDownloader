using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParalelDownloader.src.Config
{
    public class AppConfig
    {
        public string[] UrlsToDownload { get; set; } = new[]
        {
            "https://upload.wikimedia.org/wikipedia/commons/thumb/4/4d/Cat_November_2010-1a.jpg/500px-Cat_November_2010-1a.jpg",
        };

        /// <summary>
        /// Vrátí počet workerů dle počtu CPU jader a počtu URL.
        /// </summary>
        public int CalculateOptimalWorkers()
        {
            int cpuCores = Environment.ProcessorCount;
            int urlCount = UrlsToDownload.Length;

            return Math.Min(cpuCores, urlCount);
        }
    }
}
