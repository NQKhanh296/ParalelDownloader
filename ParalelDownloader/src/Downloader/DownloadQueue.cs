using System.Collections.Concurrent;

namespace ParalelDownloader.src.Downloader
{
    /// <summary>
    /// A thread-safe download task queue implemented using ConcurrentQueue.
    /// Workers retrieve tasks from this queue without requiring explicit locking.
    /// </summary>
    public class DownloadQueue
    {
        private readonly ConcurrentQueue<DownloadTask> _queue = new();

        /// <summary>
        /// Adds a download task to the queue.
        /// </summary>
        /// <param name="task">The task to enqueue.</param>
        public void Enqueue(DownloadTask task)
        {
            _queue.Enqueue(task);
        }

        /// <summary>
        /// Attempts to remove a task from the queue.
        /// </summary>
        /// <param name="task">The dequeued task, if available.</param>
        /// <returns>True if a task was successfully dequeued; otherwise false.</returns>
        public bool TryDequeue(out DownloadTask task)
        {
            return _queue.TryDequeue(out task);
        }

        /// <summary>
        /// Indicates whether the queue is empty.
        /// </summary>
        /// <returns>True if the queue contains no tasks; otherwise false.</returns>
        public bool IsEmpty => _queue.IsEmpty;
    }
}
