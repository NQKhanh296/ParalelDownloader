namespace ParalelDownloader.src.Downloader
{
    /// <summary>
    /// Reprezentuje jeden úkol ke stažení:
    ///  - URL zdroje
    ///  - absolutní cestu výsledného souboru
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
