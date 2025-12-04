using ParalelDownloader.src.Config;
using ParalelDownloader.src.Core;
using ParalelDownloader.src.Downloader;


namespace ParalelDownloader.src.UI
{
    /// <summary>
    /// Provides the console-based user interface of the application.
    /// Handles the main menu, user input, and management of the target download folder.
    /// </summary>
    public class MainMenu
    {
        private string _currentOutputFolder;

        public MainMenu()
        {
            _currentOutputFolder = PathConfig.LoadSavedFolderOrDefault();
        }

        /// <summary>
        /// Runs the main menu loop, displaying options and processing user choices.
        /// </summary>
        /// <returns>A task representing the asynchronous menu loop.</returns>
        public async Task RunAsync()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("===== PARALLEL DOWNLOADER =====");
                Console.WriteLine($"Target folder: {_currentOutputFolder}");
                Console.WriteLine();
                Console.WriteLine("1) Download files from URL");
                Console.WriteLine("2) Change target folder");
                Console.WriteLine("3) Exit program");
                Console.WriteLine();
                Console.Write("Choose (1–3): ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": await HandleDownloadAsync(); break;
                    case "2": HandleChangeFolder(); break;
                    case "3": return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        Console.ReadKey(true);
                        break;
                }
            }
        }

        /// <summary>
        /// Handles the download workflow:
        /// - Prompts the user for URLs
        /// - Validates input
        /// - Configures worker count
        /// - Produces download tasks and launches workers
        /// - Displays results after completion
        /// </summary>
        private async Task HandleDownloadAsync()
        {
            Console.Clear();
            Console.WriteLine("=== FILE DOWNLOAD ===");
            Console.WriteLine($"Target folder: {_currentOutputFolder}");
            Console.WriteLine("(ENTER = back)");
            Console.WriteLine();

            Console.Write("Enter URLs separated by commas: ");
            string? urlInput = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(urlInput))
                return;

            string[] urls = urlInput.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);


            if (urls.Length == 0)
            {
                Console.WriteLine("No valid URLs were entered.");
                Console.ReadKey(true);
                return;
            }

            if (!PathConfig.CanWriteToFolder(_currentOutputFolder, out var error))
            {
                Console.WriteLine($"Invalid folder: {_currentOutputFolder}");
                Console.WriteLine($"Error: {error}");
                Console.ReadKey(true);
                return;
            }

            var config = new AppConfig();
            int workerCount = config.CalculateOptimalWorkers(urls.Length);

            Console.WriteLine();
            Console.WriteLine($"CPU cores: {Environment.ProcessorCount}");
            Console.WriteLine($"Number of URLs: {urls.Length}");
            Console.WriteLine($"Workers in thread pool: {workerCount}");
            Console.WriteLine();

            DownloadStatistics.Reset();

            var queue = new DownloadQueue();
            var producer = new TaskProducer(queue, _currentOutputFolder);
            producer.ProduceTasks(urls);

            var semaphore = new SemaphoreSlim(workerCount);
            var factory = new WorkerFactory();
            var workerTasks = new List<Task>();

            for (int i = 1; i <= workerCount; i++)
            {
                var worker = factory.CreateWorker(i, queue, semaphore, _currentOutputFolder);
                workerTasks.Add(worker.StartAsync());
            }

            await Task.WhenAll(workerTasks);

            Console.WriteLine();
            Console.WriteLine($"Successfully downloaded: {DownloadStatistics.SuccessCount} z {urls.Length}");
            Console.WriteLine("Press any key...");
            Console.ReadKey(true);
        }

        /// <summary>
        /// Handles changing the download output folder.
        /// Allows the user to specify a new folder or reset to the default Downloads directory.
        /// </summary>
        private void HandleChangeFolder()
        {
            while (true)
            {
                Console.Clear();

                string defaultFolder = PathConfig.GetDefaultFolder();

                Console.WriteLine("=== CHANGE TARGET FOLDER ===");
                Console.WriteLine($"Current folder: {_currentOutputFolder}");
                Console.WriteLine();
                Console.WriteLine("1) Enter new folder");
                Console.WriteLine("2) Reset to default (Downloads)");
                Console.WriteLine("3) Back to menu");
                Console.WriteLine();
                Console.Write("Choose (1-3): ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("Enter path (ENTER = back): ");
                        string? newPath = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(newPath))
                            continue;

                        if (!PathConfig.TrySaveFolder(newPath, out var error))
                        {
                            Console.WriteLine($"Error: {error}");
                            Console.WriteLine("Press any key...");
                            Console.ReadKey(true);
                        }
                        else
                        {
                            _currentOutputFolder = newPath;
                            Console.WriteLine("Target folder has been changed.");
                            Console.ReadKey(true);
                        }
                        break;

                    case "2":
                        if (PathConfig.TrySaveFolder(defaultFolder, out var err))
                        {
                            _currentOutputFolder = defaultFolder;
                            Console.WriteLine("Folder reset to default.");
                        }
                        else
                        {
                            Console.WriteLine($"Error: {err}");
                        }
                        Console.ReadKey(true);
                        break;

                    case "3":
                        return;

                    default:
                        Console.WriteLine("Invalid choice.");
                        Console.ReadKey(true);
                        break;
                }
            }
        }
    }
}
