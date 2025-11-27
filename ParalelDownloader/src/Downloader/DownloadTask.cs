using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParalelDownloader.src.Downloader
{
    internal class DownloadTask
    {
        public string Url { get; }
        public string FileName { get; }

        public DownloadTask(string url, string fileName)
        {
            Url = url;
            FileName = fileName;
        }
    }
}
