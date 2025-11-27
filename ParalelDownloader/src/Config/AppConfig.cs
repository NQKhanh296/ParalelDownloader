using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParalelDownloader.src.Config
{
    internal class AppConfig
    {
        public int WorkerCount { get; set; } = 3;

        public string[] UrlsToDownload { get; set; } = new[]
        {
            "https://speed.hetzner.de/100MB.bin",
            "https://speed.hetzner.de/50MB.bin",
            "https://speed.hetzner.de/10MB.bin"
        };
    }
}
