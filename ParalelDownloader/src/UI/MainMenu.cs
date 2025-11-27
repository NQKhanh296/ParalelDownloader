using ParalelDownloader.src.Config;
using ParalelDownloader.src.Core;
using ParalelDownloader.src.Downloader;


namespace ParalelDownloader.src.UI
{
    /// <summary>
    /// MainMenu zajišťuje konzolové uživatelské rozhraní programu.
    /// Obsahuje hlavní menu, volby uživatele, správu cílové složky
    /// </summary>
    public class MainMenu
    {
        // Aktuální složka, kam se ukládají soubory
        private string _currentOutputFolder;

        /// <summary>
        /// Konstruktor načte uloženou složku ze souboru konfigurace
        /// nebo použije výchozí (Downloads), pokud žádná není uložená.
        /// </summary>
        public MainMenu()
        {
            _currentOutputFolder = PathConfig.LoadSavedFolderOrDefault();
        }

        /// <summary>
        /// Hlavní smyčka menu.
        /// </summary>
        public async Task RunAsync()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("===== PARALLEL DOWNLOAD MANAGER =====");
                Console.WriteLine($"Cílová složka: {_currentOutputFolder}");
                Console.WriteLine();
                Console.WriteLine("1) Stahovat soubory z URL");
                Console.WriteLine("2) Změnit cílovou složku");
                Console.WriteLine("3) Ukončit program");
                Console.WriteLine();
                Console.Write("Vyberte (1-3): ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": await HandleDownloadAsync(); break;
                    case "2": HandleChangeFolder(); break;
                    case "3": return;
                    default:
                        Console.WriteLine("Neplatná volba.");
                        Console.ReadKey(true);
                        break;
                }
            }
        }

        /// <summary>
        /// Obsluha stahování.
        /// </summary>
        private async Task HandleDownloadAsync()
        {
            Console.Clear();
            Console.WriteLine("=== STAHOVÁNÍ SOUBORŮ ===");
            Console.WriteLine($"Cílová složka: {_currentOutputFolder}");
            Console.WriteLine("(ENTER = zpět)");
            Console.WriteLine();

            Console.Write("Zadejte URL oddělené čárkou: ");
            string? urlInput = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(urlInput))
                return;

            // Převod na pole URL
            string[] urls = urlInput.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (urls.Length == 0)
            {
                Console.WriteLine("Nebyla zadána žádná platná URL.");
                Console.ReadKey(true);
                return;
            }

            // Ověření možnosti zápisu do cílové složky
            if (!PathConfig.CanWriteToFolder(_currentOutputFolder, out var error))
            {
                Console.WriteLine($"Chybná složka: {_currentOutputFolder}");
                Console.WriteLine($"Chyba: {error}");
                Console.ReadKey(true);
                return;
            }

            var config = new AppConfig();
            int workerCount = config.CalculateOptimalWorkers(urls.Length);

            Console.WriteLine();
            Console.WriteLine($"CPU jader: {Environment.ProcessorCount}");
            Console.WriteLine($"Počet URL: {urls.Length}");
            Console.WriteLine($"Workerů v thread poolu: {workerCount}");
            Console.WriteLine();

            DownloadStatistics.Reset();

            var queue = new DownloadQueue();
            var producer = new TaskProducer(queue, _currentOutputFolder);
            producer.ProduceTasks(urls);

            var semaphore = new SemaphoreSlim(workerCount);
            var factory = new WorkerFactory();
            var workerTasks = new List<Task>();

            // Vytvoření workerů
            for (int i = 1; i <= workerCount; i++)
            {
                var worker = factory.CreateWorker(i, queue, semaphore, _currentOutputFolder);
                workerTasks.Add(worker.StartAsync());
            }

            await Task.WhenAll(workerTasks);

            Console.WriteLine();
            Console.WriteLine($"Úspěšně staženo: {DownloadStatistics.SuccessCount} z {urls.Length}");
            Console.WriteLine("Stiskněte libovolnou klávesu...");
            Console.ReadKey(true);
        }

        /// <summary>
        /// Obsluha změny cílové složky.
        /// </summary>
        private void HandleChangeFolder()
        {
            while (true)
            {
                Console.Clear();

                string defaultFolder = PathConfig.GetDefaultFolder();

                Console.WriteLine("=== ZMĚNA CÍLOVÉ SLOŽKY ===");
                Console.WriteLine($"Současná složka: {_currentOutputFolder}");
                Console.WriteLine();
                Console.WriteLine("1) Zadat novou složku");
                Console.WriteLine("2) Reset na výchozí (Downloads)");
                Console.WriteLine("3) Zpět do menu");
                Console.WriteLine();
                Console.Write("Vyberte (1-3): ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("Zadejte cestu (ENTER = zpět): ");
                        string? newPath = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(newPath))
                            continue;

                        if (!PathConfig.TrySaveFolder(newPath, out var error))
                        {
                            Console.WriteLine($"Chyba: {error}");
                            Console.WriteLine("Stiskněte klávesu...");
                            Console.ReadKey(true);
                        }
                        else
                        {
                            _currentOutputFolder = newPath;
                            Console.WriteLine("Cílová složka byla změněna.");
                            Console.ReadKey(true);
                        }
                        break;

                    case "2":
                        if (PathConfig.TrySaveFolder(defaultFolder, out var err))
                        {
                            _currentOutputFolder = defaultFolder;
                            Console.WriteLine("Složka resetována na výchozí.");
                        }
                        else
                        {
                            Console.WriteLine($"Chyba: {err}");
                        }
                        Console.ReadKey(true);
                        break;

                    case "3":
                        return;

                    default:
                        Console.WriteLine("Neplatná volba.");
                        Console.ReadKey(true);
                        break;
                }
            }
        }
    }
}
