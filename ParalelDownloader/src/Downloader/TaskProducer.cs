using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParalelDownloader.src.Downloader
{
    internal class TaskProducer
    {
        private readonly DownloadQueue _queue;

        public TaskProducer(DownloadQueue queue)
        {
            _queue = queue;
        }

        /// <summary>
        /// Převede seznam URL na úkoly a vloží je do fronty.
        /// </summary>
        public void ProduceTasks(string[] urls)
        {
            int index = 1;

            foreach (var url in urls)
            {
                // Vytvoříme název souboru z indexu
                string fileName = $"soubor_{index}.bin";

                // Vytvoříme úkol
                var task = new DownloadTask(url, fileName);

                // Přidáme do fronty
                _queue.Enqueue(task);

                Console.WriteLine($"[PRODUCENT] Přidán úkol: {url} → {fileName}");

                index++;
            }
        }
    }
}
