using DAL.Shared.Models;
using SDAConsole.types;
using SDAConsole.interfaces.ConfigHub;
using SDAConsole.types.ConfigHub;
using System.Text.Json;
using ShellProgressBar;

namespace SDAConsole.loaders
{
    public class DalConfigurationLoader
    {
        public static DalConfiguration LoadFromFile(string filePath, IProgressBar? parent = null)
        {
            using var sub = new ProgressHelper($"Reading: {Path.GetFileName(filePath)}", 1, parent as ProgressBar);

            if (!File.Exists(filePath))
            {
                sub.WriteLine($"{filePath} File not found");
                throw new FileNotFoundException("Missing", filePath);
            }

            var json = File.ReadAllText(filePath);
            sub.Tick($"{filePath} Read complete");

            return JsonSerializer.Deserialize<DalConfiguration>(json)!;
        }

    }

    public class TestObjectConfigurationLoader
    {
        public static TestObjectConfiguration LoadFromFile(string filePath, IProgressBar? parent = null)
        {
            using var sub = new ProgressHelper($"Reading: {Path.GetFileName(filePath)}", 1, parent as ProgressBar);

            if (!File.Exists(filePath))
                {
                    sub.WriteLine($"{filePath} File not found");
                    throw new FileNotFoundException("Configuration file not found", filePath);
                }

            string json = File.ReadAllText(filePath);

            sub.Tick($"{filePath} Read complete");

            return JsonSerializer.Deserialize<TestObjectConfiguration>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new InvalidOperationException("Failed to deserialize TestObjectConfiguration.");
        }
    }

    public class EcuListLoader
    {
        public static List<ECUList> LoadFromFile(string filePath, IProgressBar? parent = null)
        {
            using var sub = new ProgressHelper($"Reading: {Path.GetFileName(filePath)}", 1, parent as ProgressBar);

            if (!File.Exists(filePath))
            {
                sub.WriteLine($"{filePath} File not found");
                throw new FileNotFoundException("Configuration file not found", filePath);
            }

            string json = File.ReadAllText(filePath);
            sub.Tick($"{filePath} Read complete");

            return JsonSerializer.Deserialize<List<ECUList>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new InvalidOperationException("Failed to deserialize ECUList.");
        }
    }
    public class VehicleTypeCodesLoader
    {
        public static List<VehicleTypeCodes> LoadFromFile(string filePath, IProgressBar? parent = null)
        {
            using var sub = new ProgressHelper($"Reading: {Path.GetFileName(filePath)}", 1, parent as ProgressBar);

            if (!File.Exists(filePath))
            {
                sub.WriteLine($"{filePath} File not found");
                throw new FileNotFoundException("Configuration file not found", filePath);
            }

            string json = File.ReadAllText(filePath);
            sub.Tick($"{filePath} Read complete");

            return JsonSerializer.Deserialize<List<VehicleTypeCodes>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new InvalidOperationException("Failed to deserialize VehicleTypeCodes.");
        }
    }

    public class InstallationReportLoader
    {
        public static ReadoutReport LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Configuration file not found", filePath);

            string json = File.ReadAllText(filePath);

            try // TODO: Is a try-catch block needed here?
            {
                return JsonSerializer.Deserialize<ReadoutReport>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? throw new InvalidOperationException("Failed to deserialize InstallationReport.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error deserializing InstallationReport: {ex.Message}", ex);
            }
        }
    }

    public class SDAConfigurationLoader
    {
        public static SDAConfiguration LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Configuration file not found", filePath);

            string json = File.ReadAllText(filePath);

            return JsonSerializer.Deserialize<SDAConfiguration>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new InvalidOperationException("Failed to deserialize SDAConfiguration.");
        }
    }

    public class ConfigHubConfigurationLoader
    {
        public static ConfigHubConfiguration LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Configuration file not found", filePath);

            string json = File.ReadAllText(filePath);

            return JsonSerializer.Deserialize<ConfigHubConfiguration>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new InvalidOperationException("Failed to deserialize ConfigHubConfiguration.");
        }
    }

}