using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PaintingApp.Contracts;
using PaintingApp.Data;
using PaintingApp.Data.Repositories.Implementations;
using PaintingApp.Data.Repositories.Interfaces;
using PaintingApp.Helpers;
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
            services.AddSingleton<IProfileStateService, ProfileStateService>();
            services.AddSingleton<ViewModelLocator>();

            return services;
        }

        public static IServiceCollection AddViewModels(this IServiceCollection services)
        {
            services.AddTransient<MainScreenViewModel>();
            services.AddTransient<ManagementViewModel>();

            return services;
        }

        public static IServiceCollection AddViews(this IServiceCollection services)
        {
            services.AddTransient<MainScreenView>();
            services.AddTransient<ManagementView>();
            services.AddTransient<DrawingView>();

            return services;
        }

        public static IServiceCollection AddDataServices(this IServiceCollection services)
        {
            var dbPath = AppPaths.GetDatabasePath();

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite($"Data Source={dbPath}"));

            services.AddScoped<IProfileRepository, ProfileRepository>();

            return services;
        }
    }
}
