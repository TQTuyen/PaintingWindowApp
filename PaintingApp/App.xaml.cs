using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using PaintingApp.Contracts;
using PaintingApp.Data;
using PaintingApp.Extensions;
using PaintingApp.ViewModels;
using Windows.Storage;

namespace PaintingApp
{
    public partial class App : Application
    {
        public new static App Current => (App)Application.Current;

        public static IServiceProvider Services { get; private set; } = null!;

        public static Window? MainWindow { get; private set; }

        public App()
        {
            InitializeComponent();

            // Configure database path for MSIX sandboxed storage before any DbContext usage
            ConfigureDatabasePath();

            Services = ConfigureServices();

            Debug.WriteLine("App: Application initialized with DI container.");
        }

        private static void ConfigureDatabasePath()
        {
            DatabasePathProvider.Configure(() =>
            {
                var localFolder = ApplicationData.Current.LocalFolder.Path;
                var dbPath = Path.Combine(localFolder, "app.db");
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

            var viewModelLocator = provider.GetService<ViewModelLocator>();
            Debug.WriteLine($"  - ViewModelLocator: {(viewModelLocator != null ? "✓ Registered" : "✗ Missing")}");

            Debug.WriteLine("App: Service validation complete.");
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
                    SetupXamlRoot(rootElement);
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
                SetupXamlRoot(rootElement);
            }
        }

        private static void SetupXamlRoot(FrameworkElement rootElement)
        {
            var dialogService = Services.GetService<IDialogService>();
            if (dialogService != null)
            {
                dialogService.XamlRoot = rootElement.XamlRoot;
                Debug.WriteLine("App: XamlRoot set for DialogService.");
            }
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
}
