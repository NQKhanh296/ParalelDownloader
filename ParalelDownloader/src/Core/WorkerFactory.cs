using ParalelDownloader.src.Downloader;

namespace ParalelDownloader.src.Core
{
    /// <summary>
    /// Implements the Factory Method design pattern.
    /// Responsible for creating instances of DownloadWorker.
    /// </summary>
    public class WorkerFactory
    {
        public DownloadWorker CreateWorker(int id, DownloadQueue queue, SemaphoreSlim semaphore, string outputFolder)
        {
            return new DownloadWorker(id, queue, semaphore, outputFolder);
        }
    }
}
