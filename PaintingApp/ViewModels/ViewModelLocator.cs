using System;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace PaintingApp.ViewModels
{
    public sealed class ViewModelLocator
    {
        private readonly IServiceProvider _serviceProvider;

        public ViewModelLocator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            Debug.WriteLine("ViewModelLocator: Initialized with IServiceProvider.");
        }

        public T GetRequiredService<T>() where T : class
        {
            var service = _serviceProvider.GetRequiredService<T>();
            Debug.WriteLine($"ViewModelLocator: Resolved {typeof(T).Name}");
            return service;
        }

        public T? GetOptionalService<T>() where T : class
        {
            var service = _serviceProvider.GetService<T>();
            
            if (service != null)
            {
                Debug.WriteLine($"ViewModelLocator: Resolved optional {typeof(T).Name}");
            }
            else
            {
                Debug.WriteLine($"ViewModelLocator: Optional {typeof(T).Name} not found");
            }

            return service;
        }

        public object GetRequiredService(Type serviceType)
        {
            var service = _serviceProvider.GetRequiredService(serviceType);
            Debug.WriteLine($"ViewModelLocator: Resolved {serviceType.Name}");
            return service;
        }

        public object? GetOptionalService(Type serviceType)
        {
            var service = _serviceProvider.GetService(serviceType);

            if (service != null)
            {
                Debug.WriteLine($"ViewModelLocator: Resolved optional {serviceType.Name}");
            }
            else
            {
                Debug.WriteLine($"ViewModelLocator: Optional {serviceType.Name} not found");
            }

            return service;
        }
    }
}
