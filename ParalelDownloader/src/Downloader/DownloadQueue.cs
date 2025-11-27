using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParalelDownloader.src.Downloader
{
    public class DownloadQueue
    {
        private readonly ConcurrentQueue<DownloadTask> _queue = new ConcurrentQueue<DownloadTask>();

        public void Enqueue(DownloadTask task)
        {
            _queue.Enqueue(task);
        }

        public bool TryDequeue(out DownloadTask task)
        {
            return _queue.TryDequeue(out task);
        }

        public bool IsEmpty => _queue.IsEmpty;
    }
}
