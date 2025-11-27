using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParalelDownloader.src.Downloader
{
    internal class DownloadWorker
    {
        private readonly DownloadQueue _queue;
        private readonly SemaphoreSlim _semaphore;
        private readonly HttpClient _client = new HttpClient();
        private readonly int _id;

        public DownloadWorker(int id, DownloadQueue queue, SemaphoreSlim semaphore)
        {
            _id = id;
            _queue = queue;
            _semaphore = semaphore;
        }

        /// <summary>
        /// Startuje nekonečnou smyčku, dokud jsou úkoly k dispozici.
        /// </summary>
        public async Task StartAsync()
        {
            while (true)
            {
                // Pokusíme se získat "povolení" ke stažení dalšího souboru.
                // SemaphoreSlim omezuje počet aktivních workerů.
                await _semaphore.WaitAsync();

                try
                {
                    // Zkusíme získat úkol z queue
                    if (_queue.TryDequeue(out var task))
                    {
                        Console.WriteLine($"[Worker {_id}] Stahuji: {task.Url}");

                        try
                        {
                            // Stáhneme soubor z internetu do bajtů
                            byte[] data = await _client.GetByteArrayAsync(task.Url);

                            // Zapíšeme soubor do disku
                            await File.WriteAllBytesAsync(task.FileName, data);

                            Console.WriteLine($"[Worker {_id}] Dokončeno: {task.FileName}");
                        }
                        catch (Exception ex)
                        {
                            // Ošetření případné chyby stahování
                            Console.WriteLine($"[Worker {_id}] Chyba při stahování: {ex.Message}");
                        }
                    }
                    else
                    {
                        // Pokud není co stahovat, worker končí
                        Console.WriteLine($"[Worker {_id}] Nejsou další úkoly. Končím.");
                        return;
                    }
                }
                finally
                {
                    // Vždy uvolníme semafor
                    _semaphore.Release();
                }
            }
        }
    }
}
