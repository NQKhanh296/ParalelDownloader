namespace ParalelDownloader.src.Config
{
    /// <summary>
    /// Manages the selection and validation of the target folder used for saving downloaded files.
    /// Loads the path from a configuration file, verifies write access, and falls back to the
    /// system "Downloads" directory when necessary.
    /// </summary>
    public class PathConfig
    {
        private static string ConfigFilePath =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "download_path.txt");

        /// <summary>
        /// Returns the system default download directory for the current user.
        /// </summary>
        /// <returns>The path to the user's default Downloads folder.</returns>
        public static string GetDefaultFolder()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Downloads"
            );
        }

        /// <summary>
        /// Loads the saved folder path from the configuration file.
        /// If the folder does not exist or is not writable, the default directory is used instead.
        /// Ensures the final folder exists by creating it if necessary.
        /// </summary>
        /// <returns>A valid directory path for storing downloaded files.</returns>
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
            catch(Exception ex)
            {
                Console.WriteLine($"[PathConfig] Failed to read config file: {ex.Message}, returned to default folder");
            }

            if (!CanWriteToFolder(folder, out _))
                folder = GetDefaultFolder();

            Directory.CreateDirectory(folder);

            return folder;
        }

        /// <summary>
        /// Attempts to save the specified folder path to the configuration file.
        /// Validates that the path is absolute, exists, and is writable before saving.
        /// </summary>
        /// <param name="folder">The folder path to store.</param>
        /// <param name="error">An error message if validation fails.</param>
        /// <returns>True if the folder was successfully saved; otherwise false.</returns>
        public static bool TrySaveFolder(string folder, out string error)
        {
            error = null;

            if (!Path.IsPathRooted(folder))
            {
                error = "The path must be absolute (e.g., C:\\Users\\Honza\\Documents).";
                return false;
            }

            if (!Directory.Exists(folder))
            {
                error = "The specified folder does not exist. Please create it manually outside the program.";
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
                error = "Error while saving configuration: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Checks whether the application has write permission for the specified folder.
        /// Creates and deletes a temporary file to verify access.
        /// </summary>
        /// <param name="folder">The folder path to validate.</param>
        /// <param name="error">Contains an error message if the check fails.</param>
        /// <returns>True if writing is possible; otherwise false.</returns>
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
