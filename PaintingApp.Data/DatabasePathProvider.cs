namespace PaintingApp.Data;

public static class DatabasePathProvider
{
    private static Func<string>? _pathResolver;

    public static string GetDatabasePath()
    {
        if (_pathResolver != null)
        {
            return _pathResolver();
        }

        // Fallback for design-time (EF migrations) when resolver is not configured
        return GetDesignTimePath();
    }

    public static void Configure(Func<string> resolver)
    {
        _pathResolver = resolver;
    }

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
