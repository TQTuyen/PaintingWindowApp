using System;
using System.IO;
using Windows.Storage;

namespace PaintingApp.Helpers;

public static class AppPaths
{
    public static string GetDatabasePath()
    {
        var localFolder = ApplicationData.Current.LocalFolder.Path;
        return Path.Combine(localFolder, "app.db");
    }

    public static string GetLocalFolderPath()
    {
        return ApplicationData.Current.LocalFolder.Path;
    }
}
