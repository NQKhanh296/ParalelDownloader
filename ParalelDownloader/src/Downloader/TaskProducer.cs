using System;
using System.IO;

namespace ParalelDownloader.src.Downloader
{
    /// <summary>
    /// Represents the PRODUCER in the Producer–Consumer model.
    /// Its responsibility is to transform the list of user-provided URLs
    /// into individual DownloadTask objects and enqueue them for worker threads.
    /// </summary>
    public class TaskProducer
    {
        private readonly DownloadQueue _queue;

        private readonly string _outputFolder;

        public TaskProducer(DownloadQueue queue, string outputFolder)
        {
            _queue = queue;
            _outputFolder = outputFolder;
        }

        /// <summary>
        /// Processes all provided URLs by:
        /// 1) Normalizing the URL.
        /// 2) Determining the file extension.
        /// 3) Generating a unique file name.
        /// 4) Constructing the full output path.
        /// 5) Enqueuing a DownloadTask for parallel processing.
        /// </summary>
        /// <param name="urls">Array of URLs entered by the user.</param>
        public void ProduceTasks(string[] urls)
        {
            foreach (var rawUrl in urls)
            {
                string url = rawUrl.Trim();

                if (string.IsNullOrWhiteSpace(url))
                    continue; 

                string extension = Path.GetExtension(url);

                if (string.IsNullOrEmpty(extension))
                    extension = ".bin";

                string fileName = $"soubor_{GenerateGuidFileName()}{extension}";

                string fullPath = Path.Combine(_outputFolder, fileName);

                var task = new DownloadTask(url, fullPath);
                _queue.Enqueue(task);
            }
        }

        /// <summary>
        /// Generates a unique file name using a GUID.
        /// </summary>
        /// <returns>A unique file name string.</returns>
        private string GenerateGuidFileName()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
