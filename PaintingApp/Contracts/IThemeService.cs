using Microsoft.UI.Xaml;

namespace PaintingApp.Contracts;

public interface IThemeService
{
    ElementTheme CurrentTheme { get; }

    void SetTheme(string themeName);

    void ApplySystemTheme();

    string GetThemeName();

    ElementTheme GetElementTheme(string themeName);
}
