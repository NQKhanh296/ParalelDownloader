using ParalelDownloader.src.Downloader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParalelDownloader.src.Core
{
    public class WorkerFactory
    {
        public DownloadWorker CreateWorker(int id, DownloadQueue queue, SemaphoreSlim semaphore)
        {
            return new DownloadWorker(id, queue, semaphore);
        }
    }
}
