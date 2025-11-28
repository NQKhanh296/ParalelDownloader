using ParalelDownloader.src.Core;

namespace ParalelDownloader.src.Downloader
{
    /// <summary>
    /// DownloadWorker představuje jedno pracovní vlákno v rámci thread poolu.
    /// Jeho úkolem je odebírat úkoly z fronty a stahovat je.
    /// Počet aktivních workerů je řízen semaforem, aby nedošlo k přetížení CPU.
    /// </summary>
    public class DownloadWorker
    {
        private readonly int _id;
        private readonly DownloadQueue _queue;
        private readonly SemaphoreSlim _semaphore;
        private readonly string _outputFolder;

        /// <summary>
        /// Konstruktor vytvoří nový worker, který bude zpracovávat úkoly.
        /// </summary>
        public DownloadWorker(int id, DownloadQueue queue, SemaphoreSlim semaphore, string outputFolder)
        {
            _id = id;
            _queue = queue;
            _semaphore = semaphore;
            _outputFolder = outputFolder;
        }

        /// <summary>
        /// Spustí pracovní smyčku workeru.
        /// Worker:
        /// 1) čeká na povolení od semaforu
        /// 2) vezme úkol z fronty
        /// 3) pokusí se stáhnout soubor
        /// 4) uloží ho (pokud se podaří)
        /// 5) uvolní místo v semaforu pro další vlákno
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

                    Console.WriteLine($"[Worker {_id}] Stahuji: {task.Url}");

                    try
                    {
                        byte[] data = await HttpClientSingleton.Instance.GetByteArrayAsync(task.Url);

                        await File.WriteAllBytesAsync(task.OutputPath, data);
                        Console.WriteLine($"[Worker {_id}] Dokončeno: {task.OutputPath}");

                        DownloadStatistics.IncrementSuccess();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Worker {_id}] CHYBA u URL: {task.Url}");
                        Console.WriteLine($"[Worker {_id}] Detail: {ex.Message}");
                        Console.WriteLine($"[Worker {_id}] Soubor nebude uložen.");
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
