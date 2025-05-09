using Microsoft.Extensions.Logging;
using ShellProgressBar;

namespace SDAConsole.FileSystemHandlers
{
    public class DirectoriesHandler
    {
        private readonly ILogger _logger;
        private readonly IProgressBar? _parent;

        public DirectoriesHandler(ILogger logger, IProgressBar? parent = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _parent = parent;

        }
        public void EnsureDirectoryAndClearFiles(string dataStorePath, string excludeFileNamesContaining = "")
        {

            if (Directory.Exists(dataStorePath))
            {
                var files = Directory.GetFiles(dataStorePath);
                using var sub = new ProgressHelper($"Solving for Path: {Path.GetFileName(dataStorePath)}", files.Length+1, _parent as ProgressBar);

                foreach (var file in files)
                {
                    if (file.Contains(excludeFileNamesContaining) && !string.IsNullOrEmpty(excludeFileNamesContaining))
                    {
                        sub.Tick($"{file} Excluded from deletion");
                        continue;
                    }else
                    {
                        File.Delete(file);
                        sub.Tick($"{file} Deleted");
                    }

                }
                sub.Tick($"{dataStorePath} Cleanup Directory complete");
            }
            else
            {
                using var sub = new ProgressHelper($"Solving for Path: {Path.GetFileName(dataStorePath)}", 1, _parent as ProgressBar);

                Directory.CreateDirectory(dataStorePath);
                sub.Tick($"{dataStorePath} Create Directory complete");
            }
        }
    }
}