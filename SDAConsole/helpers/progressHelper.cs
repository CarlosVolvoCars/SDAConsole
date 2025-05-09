using ShellProgressBar;

public class ProgressHelper : IDisposable
{
    private readonly IProgressBar _mainBar;
    private readonly ProgressBar? _parent;
    private readonly Timer _refreshTimer;

    private readonly int _maxTicks = 100; // Default value, can be changed in constructor

    public ProgressHelper(string title, int maxTicks, ProgressBar? parent = null)
    {
        _parent = parent;
        _maxTicks = maxTicks;

        var options = new ProgressBarOptions
        {
            ForegroundColor = parent != null ? ConsoleColor.Cyan : ConsoleColor.Yellow,
            ProgressCharacter = '•',
            ProgressBarOnBottom = true,
            CollapseWhenFinished = false,
            DisplayTimeInRealTime = false
        };

        _mainBar = parent != null
            ? parent.Spawn(maxTicks, title, options)
            : new ProgressBar(maxTicks, title, options);

        // Start a timer to refresh display every second
        _refreshTimer = new Timer(_ =>
        {
            // Re-render without incrementing
            _mainBar.Tick(_mainBar.CurrentTick);
        }, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
    }

    // Method to update the progress bar to its full value
    public void Complete(string message = "")
    {
        _mainBar.Tick(_maxTicks, message);
        _mainBar.Dispose();
    }
    public void Tick(string message = "")
    {
        System.Threading.Thread.Sleep(20); // Add a delay to allow visualization if proceses is too fast
        _mainBar.Tick(message);
    }

    public void WriteLine(string message) => _mainBar.WriteLine(message);

    public void Dispose()
    {

        _refreshTimer.Dispose();
        _mainBar.Dispose();
    }

    public IProgressBar Bar => _mainBar;
    public ProgressBar? Parent => _parent;
}
