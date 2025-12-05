namespace PaintingApp.Data;

/// <summary>
/// Provides database path resolution for different runtime contexts.
/// </summary>
public static class DatabasePathProvider
{
    private static Func<string>? _pathResolver;

    /// <summary>
    /// Gets the database file path using the configured resolver, or falls back to design-time path.
    /// </summary>
    public static string GetDatabasePath()
    {
        if (_pathResolver != null)
        {
            return _pathResolver();
        }

        // Fallback for design-time (EF migrations) when resolver is not configured
        return GetDesignTimePath();
    }

    /// <summary>
    /// Configures the path resolver for runtime use. Call this from App.xaml.cs during startup.
    /// </summary>
    /// <param name="resolver">A function that returns the database directory path.</param>
    public static void Configure(Func<string> resolver)
    {
        _pathResolver = resolver;
    }

    /// <summary>
    /// Gets the design-time path for EF Core migrations (non-MSIX context).
    /// Uses project directory for design-time tooling compatibility.
    /// </summary>
    internal static string GetDesignTimePath()
    {
        // For design-time (EF migrations), use a relative path in the project output
        var baseDirectory = AppContext.BaseDirectory;
        var directory = Path.Combine(baseDirectory, "Data");

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        return Path.Combine(directory, "app.db");
    }
}
