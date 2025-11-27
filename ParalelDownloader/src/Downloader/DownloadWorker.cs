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
        // Identifikační číslo workeru 
        private readonly int _id;

        // Sdílená fronta úkolů 
        private readonly DownloadQueue _queue;

        // Semafor omezuje počet současně běžících workerů
        private readonly SemaphoreSlim _semaphore;

        // Složka, kam worker ukládá stažené soubory
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
                // Čekání, dokud není volná kapacita (thread pool limit)
                await _semaphore.WaitAsync();

                try
                {
                    // Pokus o získání dalšího úkolu
                    if (!_queue.TryDequeue(out var task))
                        return; 

                    Console.WriteLine($"[Worker {_id}] Stahuji: {task.Url}");

                    try
                    {
                        // Stahování souboru 
                        byte[] data = await HttpClientSingleton.Instance.GetByteArrayAsync(task.Url);

                        // Uložení souboru — pouze při úspěšném stažení
                        await File.WriteAllBytesAsync(task.OutputPath, data);
                        Console.WriteLine($"[Worker {_id}] Dokončeno: {task.OutputPath}");

                        // Zaznamenání úspěšného stažení
                        DownloadStatistics.IncrementSuccess();
                    }
                    catch (Exception ex)
                    {
                        // Chytání chyb během stahování nebo ukládání
                        Console.WriteLine($"[Worker {_id}] CHYBA u URL: {task.Url}");
                        Console.WriteLine($"[Worker {_id}] Detail: {ex.Message}");
                        Console.WriteLine($"[Worker {_id}] Soubor nebude uložen.");
                    }
                }
                finally
                {
                    // Worker uvolní místo v semaforu
                    _semaphore.Release();
                }
            }
        }
    }
}
