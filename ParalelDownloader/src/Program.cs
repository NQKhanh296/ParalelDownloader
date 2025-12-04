using ParalelDownloader.src.UI;

namespace ParalelDownloader.src
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var menu = new MainMenu();
            await menu.RunAsync();
        }
    }
}
