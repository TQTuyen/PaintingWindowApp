using System;
using Microsoft.UI.Xaml.Controls;

namespace PaintingApp.Contracts
{
    public interface INavigationService
    {
        event EventHandler<Type>? Navigated;

        bool CanGoBack { get; }

        Frame? Frame { get; set; }

        bool NavigateTo<T>(object? parameter = null) where T : class;

        bool NavigateTo(Type pageType, object? parameter = null);

        bool NavigateTo(string pageKey, object? parameter = null);

        bool GoBack();

        void ClearBackStack();

        void RegisterPage(string key, Type pageType);

        void UnregisterPage(string key);
    }
}
