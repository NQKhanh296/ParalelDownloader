using System;
using System.IO;

namespace ParalelDownloader.src.Downloader
{
    /// <summary>
    /// TaskProducer je PRODUCENT v modelu Producer–Consumer.
    /// Jeho úkolem je převést seznam URL od uživatele na jednotlivé úkoly (DownloadTask)
    /// a vložit je do sdílené fronty pro download worker vlákna.
    /// </summary>
    public class TaskProducer
    {
        // Sdílená fronta
        private readonly DownloadQueue _queue;

        // Složka, do které se budou stahovat soubory
        private readonly string _outputFolder;

        /// <summary>
        /// Konstruktor producenta
        /// </summary>
        public TaskProducer(DownloadQueue queue, string outputFolder)
        {
            _queue = queue;
            _outputFolder = outputFolder;
        }

        /// <summary>
        /// Zpracuje všechna URL:
        /// 1) Normalizuje je
        /// 2) Zjistí příponu z URL
        /// 3) Vytvoří název souboru
        /// 4) Vygeneruje kompletní výstupní cestu
        /// 5) Vloží úkol do fronty pro paralelní stažení
        /// </summary>
        public void ProduceTasks(string[] urls)
        {
            int index = 1; 

            foreach (var rawUrl in urls)
            {
                string url = rawUrl.Trim();

                if (string.IsNullOrWhiteSpace(url))
                    continue; 

                // Pokusí se z URL získat příponu
                string extension = Path.GetExtension(url);

                // Pokud URL žádnou příponu nemá, použije .bin
                if (string.IsNullOrEmpty(extension))
                    extension = ".bin";

                // Název souboru
                string fileName = $"soubor_{index}{extension}";

                // Kompletní cesta: složka + název souboru
                string fullPath = Path.Combine(_outputFolder, fileName);

                // Nový úkol pro stažení
                var task = new DownloadTask(url, fullPath);
                _queue.Enqueue(task);

                Console.WriteLine($"[PRODUCENT] Přidán úkol: {url} → {fullPath}");

                index++; 
            }
        }
    }
}
