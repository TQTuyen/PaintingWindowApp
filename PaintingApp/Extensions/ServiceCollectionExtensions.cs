using Microsoft.Extensions.DependencyInjection;
using PaintingApp.Contracts;

namespace PaintingApp.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            return services;
        }

        public static IServiceCollection AddViewModels(this IServiceCollection services)
        {
            return services;
        }

        public static IServiceCollection AddViews(this IServiceCollection services)
        {
            return services;
        }

        public static IServiceCollection AddDataServices(this IServiceCollection services)
        {
            return services;
        }
    }
}
