using ParalelDownloader.src.Downloader;

namespace ParalelDownloader.src.Core
{
    /// <summary>
    /// Implementace návrhového vzoru Factory Method.
    /// Slouží k vytváření objektů DownloadWorker
    /// </summary>
    public class WorkerFactory
    {
        public DownloadWorker CreateWorker(int id, DownloadQueue queue, SemaphoreSlim semaphore, string outputFolder)
        {
            return new DownloadWorker(id, queue, semaphore, outputFolder);
        }
    }
}
