using InteractiveApp.Config;

namespace InteractiveApp.Services
{
    public static class RestoreService
    {

        public static void RestoreBackup(ConfigManager _configManager)
        {
            string backupPath = "./configs/configuration.backup.json";
            string currentPath = "./configs/configuration.json";

            if (File.Exists(backupPath))
            {
                File.Copy(backupPath, currentPath, overwrite: true);
                _configManager.ReloadConfig();
                Console.WriteLine("Configuration restored.");
            }
            else
            {
                Console.WriteLine("No backup found.");
            }
            Console.ReadKey();
        }
    }

}
