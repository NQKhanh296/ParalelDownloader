using ParalelDownloader.src.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParalelDownloader.src.Downloader
{
    public class DownloadWorker
    {
        private readonly int _id;
        private readonly DownloadQueue _queue;
        private readonly SemaphoreSlim _semaphore;

        public DownloadWorker(int id, DownloadQueue queue, SemaphoreSlim semaphore)
        {
            _id = id;
            _queue = queue;
            _semaphore = semaphore;
        }

        /// <summary>
        /// Hlavní smyčka workeru (konzumenta).
        /// Worker pracuje, dokud nejsou všechny úkoly pryč.
        /// </summary>
        public async Task StartAsync()
        {
            while (true)
            {
                // Povolí vstup do kritické sekce max. podle počtu jader
                await _semaphore.WaitAsync();

                try
                {
                    // Zkus si vzít úkol
                    if (!_queue.TryDequeue(out DownloadTask task))
                    {
                        // Už není co dělat → worker končí
                        return;
                    }

                    Console.WriteLine($"[Worker {_id}] Stahuji: {task.Url}");

                    try
                    {
                        // Singleton HttpClient
                        var data = await HttpClientSingleton.Instance.GetByteArrayAsync(task.Url);

                        await File.WriteAllBytesAsync(task.FileName, data);

                        Console.WriteLine($"[Worker {_id}] Dokončeno: {task.FileName}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Worker {_id}] CHYBA: {ex.Message}");
                    }
                }
                finally
                {
                    // Worker se uvolnil a hned může pracovat dál
                    _semaphore.Release();
                }
            }
        }
    }
}
