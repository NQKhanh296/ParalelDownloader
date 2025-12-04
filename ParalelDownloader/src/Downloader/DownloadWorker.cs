using ParalelDownloader.src.Core;

namespace ParalelDownloader.src.Downloader
{
    /// <summary>
    /// Represents a single worker thread within the thread pool.
    /// A worker continuously retrieves download tasks from the shared queue,
    /// downloads the file, saves it, and reports completion.
    /// The number of simultaneously active workers is controlled by a semaphore
    /// to prevent CPU overload.
    /// </summary>
    public class DownloadWorker
    {
        private readonly int _id;
        private readonly DownloadQueue _queue;
        private readonly SemaphoreSlim _semaphore;
        private readonly string _outputFolder;

        public DownloadWorker(int id, DownloadQueue queue, SemaphoreSlim semaphore, string outputFolder)
        {
            _id = id;
            _queue = queue;
            _semaphore = semaphore;
            _outputFolder = outputFolder;
        }

        /// <summary>
        /// Starts the worker's processing loop.
        /// 1) Waits for semaphore permission (limits concurrency).
        /// 2) Attempts to retrieve the next task from the queue.
        /// 3) Downloads the file from the specified URL.
        /// 4) Saves the downloaded data to the output path if successful.
        /// 5) Increments global download statistics.
        /// 6) Releases the semaphore slot for another worker.
        /// The loop ends when no more tasks are available.
        /// </summary>
        public async Task StartAsync()
        {
            while (true)
            {
                await _semaphore.WaitAsync();

                try
                {
                    if (!_queue.TryDequeue(out var task))
                        return; 

                    Console.WriteLine($"[Worker {_id}] Downloading: {task.Url}");

                    try
                    {
                        byte[] data = await HttpClientSingleton.Instance.GetByteArrayAsync(task.Url);

                        await File.WriteAllBytesAsync(task.OutputPath, data);
                        Console.WriteLine($"[Worker {_id}] Completed: {task.OutputPath}");

                        DownloadStatistics.IncrementSuccess();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Worker {_id}] Error at URL: {task.Url}");
                        Console.WriteLine($"[Worker {_id}] Detail: {ex.Message}");
                        Console.WriteLine($"[Worker {_id}] File will not be saved.");
                    }
                }
                finally
                {
                    _semaphore.Release();
                }
            }
        }
    }
}
