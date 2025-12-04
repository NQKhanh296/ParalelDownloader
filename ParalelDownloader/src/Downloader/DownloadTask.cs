namespace ParalelDownloader.src.Downloader
{
    /// <summary>
    /// Represents a single download task, containing:
    /// - The source URL.
    /// - The absolute path of the output file.
    /// </summary>
    public class DownloadTask
    {
        public string Url { get; }
        public string OutputPath { get; }

        public DownloadTask(string url, string outputPath)
        {
            Url = url;
            OutputPath = outputPath;
        }
    }
}
