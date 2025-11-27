using System.Collections.Concurrent;

namespace ParalelDownloader.src.Downloader
{
    /// <summary>
    /// DownloadQueue je vlákny bezpečná fronta (ConcurrentQueue)
    /// pro ukládání úkolů ke stažení.
    /// Workeři z ní úkoly odebírají bez nutnosti zamykání (lock).
    /// </summary>
    public class DownloadQueue
    {
        private readonly ConcurrentQueue<DownloadTask> _queue = new();

        /// <summary>
        /// Přidá úkol do fronty.
        /// </summary>
        public void Enqueue(DownloadTask task)
        {
            _queue.Enqueue(task);
        }

        /// <summary>
        /// Pokusí se odebrat úkol z fronty.
        /// </summary>
        public bool TryDequeue(out DownloadTask task)
        {
            return _queue.TryDequeue(out task);
        }

        /// <summary>
        /// Vrací true, pokud je fronta prázdná.
        /// </summary>
        public bool IsEmpty => _queue.IsEmpty;
    }
}
