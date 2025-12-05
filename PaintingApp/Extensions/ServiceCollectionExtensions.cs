using Microsoft.Extensions.DependencyInjection;
using PaintingApp.Contracts;
using PaintingApp.Services;
using PaintingApp.ViewModels;
using PaintingApp.Views;

namespace PaintingApp.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<IThemeService, ThemeService>();
            services.AddSingleton<ViewModelLocator>();

            return services;
        }

        public static IServiceCollection AddViewModels(this IServiceCollection services)
        {
            services.AddTransient<ManagementViewModel>();

            return services;
        }

        public static IServiceCollection AddViews(this IServiceCollection services)
        {
            services.AddTransient<ManagementView>();

            return services;
        }

        public static IServiceCollection AddDataServices(this IServiceCollection services)
        {
            return services;
        }
    }
}
