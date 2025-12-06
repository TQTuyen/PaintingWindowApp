using System;
using System.Diagnostics;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using PaintingApp.Contracts;
using PaintingApp.Data;
using PaintingApp.Extensions;
using PaintingApp.Helpers;
using PaintingApp.ViewModels;
using Windows.Storage;

namespace PaintingApp;

public partial class App : Application
{
    public new static App Current => (App)Application.Current;

    public static IServiceProvider Services { get; private set; } = null!;

    public static Window? MainWindow { get; private set; }

    public App()
    {
        InitializeComponent();

        ConfigureDatabasePath();

        Services = ConfigureServices();

        InitializeDatabase();

        Debug.WriteLine("App: Application initialized with DI container.");
    }

    private static void ConfigureDatabasePath()
    {
        DatabasePathProvider.Configure(() =>
        {
            var dbPath = AppPaths.GetDatabasePath();
            Debug.WriteLine($"App: Database path configured: {dbPath}");
            return dbPath;
        });
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddCoreServices();
        services.AddViewModels();
        services.AddViews();
        services.AddDataServices();

        var provider = services.BuildServiceProvider();

        ValidateServices(provider);

        return provider;
    }

    private static void ValidateServices(IServiceProvider provider)
    {
        Debug.WriteLine("App: Validating registered services...");

        // Validate core services
        var navigationService = provider.GetService<INavigationService>();
        Debug.WriteLine($"  - INavigationService: {(navigationService != null ? "✓ Registered" : "✗ Missing")}");

        var dialogService = provider.GetService<IDialogService>();
        Debug.WriteLine($"  - IDialogService: {(dialogService != null ? "✓ Registered" : "✗ Missing")}");

        var themeService = provider.GetService<IThemeService>();
        Debug.WriteLine($"  - IThemeService: {(themeService != null ? "✓ Registered" : "✗ Missing")}");

        var viewModelLocator = provider.GetService<ViewModelLocator>();
        Debug.WriteLine($"  - ViewModelLocator: {(viewModelLocator != null ? "✓ Registered" : "✗ Missing")}");

        Debug.WriteLine("App: Service validation complete.");
    }

    private static void InitializeDatabase()
    {
        try
        {
            using var scope = Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            
            Debug.WriteLine("App: Running database migrations...");
            dbContext.Database.Migrate();
            Debug.WriteLine("App: Database migrations completed successfully.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"App: Database migration failed: {ex.Message}");
            throw;
        }
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        MainWindow = new MainWindow();

        // Set up XamlRoot for DialogService after window content is loaded
        if (MainWindow.Content is FrameworkElement rootElement)
        {
            rootElement.Loaded += OnRootElementLoaded;
        }
        else
        {
            // If Content is not set yet, wait for it
            MainWindow.Activated += OnMainWindowActivated;
        }

        MainWindow.Activate();

        Debug.WriteLine("App: MainWindow activated.");
    }

    private void OnMainWindowActivated(object sender, WindowActivatedEventArgs args)
    {
        if (MainWindow?.Content is FrameworkElement rootElement)
        {
            MainWindow.Activated -= OnMainWindowActivated;

            if (rootElement.IsLoaded)
            {
                SetupServices(rootElement);
            }
            else
            {
                rootElement.Loaded += OnRootElementLoaded;
            }
        }
    }

    private void OnRootElementLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement rootElement)
        {
            rootElement.Loaded -= OnRootElementLoaded;
            SetupServices(rootElement);
        }
    }

    private static void SetupServices(FrameworkElement rootElement)
    {
        var dialogService = Services.GetService<IDialogService>();
        if (dialogService != null)
        {
            dialogService.XamlRoot = rootElement.XamlRoot;
            Debug.WriteLine("App: XamlRoot set for DialogService.");
        }

        var themeService = Services.GetService<IThemeService>();
        themeService?.ApplySystemTheme();
        Debug.WriteLine("App: System theme applied.");
    }

    public static T GetService<T>() where T : class
    {
        return Services.GetRequiredService<T>();
    }

    public static T? GetOptionalService<T>() where T : class
    {
        return Services.GetService<T>();
    }
}
