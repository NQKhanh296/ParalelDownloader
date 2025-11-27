namespace ParalelDownloader.src.Core
{
    /// <summary>
    /// DownloadStatistics udržuje počet úspěšně stažených souborů.
    /// Používá thread-safe Interlocked.Increment, protože increment
    /// může probíhat paralelně z více workerů současně.
    /// </summary>
    public static class DownloadStatistics
    {
        private static int _successCount;

        /// <summary>
        /// Resetuje počítadlo úspěšných stažení (před novou dávkou).
        /// </summary>
        public static void Reset() => _successCount = 0;

        /// <summary>
        /// Zvyšuje hodnotu počítadla — volají workeři po úspěšném stažení.
        /// </summary>
        public static void IncrementSuccess()
        {
            Interlocked.Increment(ref _successCount);
        }

        /// <summary>
        /// Vrací počet úspěšných stažení.
        /// </summary>
        public static int SuccessCount => _successCount;
    }
}
