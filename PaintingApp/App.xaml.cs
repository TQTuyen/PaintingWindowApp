using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using PaintingApp.Extensions;

namespace PaintingApp
{
    public partial class App : Application
    {
        private Window? _window;

        public new static App Current => (App)Application.Current;

        public static IServiceProvider Services { get; private set; } = null!;

        public App()
        {
            InitializeComponent();

            Services = ConfigureServices();
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddCoreServices();

            return services.BuildServiceProvider();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            _window = new MainWindow();
            _window.Activate();
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
