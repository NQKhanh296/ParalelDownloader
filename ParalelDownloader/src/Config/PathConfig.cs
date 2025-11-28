namespace ParalelDownloader.src.Config
{
    /// <summary>
    /// PathConfig spravuje výběr cílové složky pro ukládání stažených souborů.
    /// - Načítá uloženou složku ze souboru download_path.txt.
    /// - Validuje, že složka existuje a lze do ní zapisovat.
    /// - Umožňuje uložit novou složku do konfigurace.
    /// - V případě chyby používá výchozí systémovou složku "Downloads".
    /// </summary>
    public class PathConfig
    {
        private static string ConfigFilePath =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "download_path.txt");

        /// <summary>
        /// Vrátí výchozí složku: uživatelův systémový "Downloads".
        /// </summary>
        public static string GetDefaultFolder()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Downloads"
            );
        }

        /// <summary>
        /// Načte uloženou složku, nebo defaultní.
        /// Ověřuje i možnost zápisu — pokud nelze zapisovat,
        /// vrátí se defaultní složka.
        /// </summary>
        public static string LoadSavedFolderOrDefault()
        {
            string folder = GetDefaultFolder();

            try
            {
                if (File.Exists(ConfigFilePath))
                {
                    string saved = File.ReadAllText(ConfigFilePath).Trim();
                    if (!string.IsNullOrWhiteSpace(saved))
                        folder = saved;
                }
            }
            catch
            {
                
            }

            if (!CanWriteToFolder(folder, out _))
                folder = GetDefaultFolder();

            Directory.CreateDirectory(folder);

            return folder;
        }

        /// <summary>
        /// Zkusí uložit danou složku jako výchozí uložiště.
        /// Před uložením ověřuje možnost zápisu.
        /// </summary>
        public static bool TrySaveFolder(string folder, out string error)
        {
            error = null;

            if (!Path.IsPathRooted(folder))
            {
                error = "Cesta musí být absolutní (např. C:\\Users\\Honza\\Documents).";
                return false;
            }

            if (!Directory.Exists(folder))
            {
                error = "Zadaná složka neexistuje. Vytvořte ji ručně mimo program.";
                return false;
            }

            if (!CanWriteToFolder(folder, out error))
            {
                return false;
            }

            try
            {
                File.WriteAllText(ConfigFilePath, folder);
                return true;
            }
            catch (Exception ex)
            {
                error = "Chyba při ukládání konfigurace: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Ověří, zda lze do dané složky zapisovat.
        /// Zkusí vytvořit a ihned smazat testovací soubor.
        /// </summary>
        public static bool CanWriteToFolder(string folder, out string error)
        {
            try
            {
                Directory.CreateDirectory(folder);

                string testFile = Path.Combine(folder, ".write_test.tmp");
                File.WriteAllText(testFile, "test");
                File.Delete(testFile);

                error = null;
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
    }
}
