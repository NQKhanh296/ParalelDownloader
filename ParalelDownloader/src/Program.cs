using ParalelDownloader.src.Config;
using ParalelDownloader.src.Core;
using ParalelDownloader.src.Downloader;
namespace ParalelDownloader.src
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("===== PARALLEL DOWNLOADER (Thread Pool + Singleton + Factory Method) =====");

            var config = new AppConfig();

            // Dynamický počet workerů podle jader
            int workerCount = config.CalculateOptimalWorkers();

            Console.WriteLine($"CPU jader: {Environment.ProcessorCount}");
            Console.WriteLine($"URL: {config.UrlsToDownload.Length}");
            Console.WriteLine($"Použiju {workerCount} workerů");

            // Sdílená fronta
            var queue = new DownloadQueue();

            // Producent vloží všechny úkoly do fronty
            var producer = new TaskProducer(queue);
            producer.ProduceTasks(config.UrlsToDownload);

            // Thread pool kontroluje počet workerů přes semafor
            var semaphore = new SemaphoreSlim(workerCount);

            // Factory method
            var factory = new WorkerFactory();

            // Spustíme worker pool
            List<Task> workerTasks = new List<Task>();

            for (int i = 1; i <= workerCount; i++)
            {
                var worker = factory.CreateWorker(i, queue, semaphore);
                workerTasks.Add(worker.StartAsync());
            }

            await Task.WhenAll(workerTasks);

            Console.WriteLine("===== HOTOVO – Vše staženo =====");
        }
    }
}
