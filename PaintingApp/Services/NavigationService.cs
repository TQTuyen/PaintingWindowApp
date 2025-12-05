using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PaintingApp.Contracts;

namespace PaintingApp.Services
{
    public sealed class NavigationService : INavigationService
    {
        private Frame? _frame;
        private readonly Dictionary<string, Type> _pageRegistry = new();

        public event EventHandler<Type>? Navigated;

        public bool CanGoBack => _frame?.CanGoBack ?? false;

        public Frame? Frame
        {
            get => _frame;
            set
            {
                if (_frame != null)
                {
                    _frame.Navigated -= OnFrameNavigated;
                }

                _frame = value;

                if (_frame != null)
                {
                    _frame.Navigated += OnFrameNavigated;
                }
            }
        }

        public bool NavigateTo<T>(object? parameter = null) where T : class
        {
            return NavigateTo(typeof(T), parameter);
        }

        public bool NavigateTo(Type pageType, object? parameter = null)
        {
            if (_frame == null)
            {
                Debug.WriteLine($"NavigationService: Frame is not set. Cannot navigate to {pageType.Name}.");
                return false;
            }

            // Don't navigate if we're already on the same page type
            if (_frame.Content?.GetType() == pageType)
            {
                Debug.WriteLine($"NavigationService: Already on page {pageType.Name}.");
                return false;
            }

            try
            {
                var result = _frame.Navigate(pageType, parameter);
                
                if (result)
                {
                    Debug.WriteLine($"NavigationService: Successfully navigated to {pageType.Name}.");
                }
                else
                {
                    Debug.WriteLine($"NavigationService: Navigation to {pageType.Name} returned false.");
                }

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"NavigationService: Navigation to {pageType.Name} failed with exception: {ex.Message}");
                return false;
            }
        }

        public bool NavigateTo(string pageKey, object? parameter = null)
        {
            if (string.IsNullOrWhiteSpace(pageKey))
            {
                Debug.WriteLine("NavigationService: Page key cannot be null or empty.");
                return false;
            }

            if (!_pageRegistry.TryGetValue(pageKey, out var pageType))
            {
                Debug.WriteLine($"NavigationService: Page key '{pageKey}' is not registered.");
                return false;
            }

            return NavigateTo(pageType, parameter);
        }

        public bool GoBack()
        {
            if (_frame == null)
            {
                Debug.WriteLine("NavigationService: Frame is not set. Cannot go back.");
                return false;
            }

            if (!_frame.CanGoBack)
            {
                Debug.WriteLine("NavigationService: Cannot go back. Back stack is empty.");
                return false;
            }

            try
            {
                _frame.GoBack();
                Debug.WriteLine("NavigationService: Successfully navigated back.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"NavigationService: GoBack failed with exception: {ex.Message}");
                return false;
            }
        }

        public void ClearBackStack()
        {
            if (_frame == null)
            {
                Debug.WriteLine("NavigationService: Frame is not set. Cannot clear back stack.");
                return;
            }

            _frame.BackStack.Clear();
            Debug.WriteLine("NavigationService: Back stack cleared.");
        }

        public void RegisterPage(string key, Type pageType)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Page key cannot be null or empty.", nameof(key));
            }

            if (pageType == null)
            {
                throw new ArgumentNullException(nameof(pageType));
            }

            if (_pageRegistry.ContainsKey(key))
            {
                Debug.WriteLine($"NavigationService: Page key '{key}' is already registered. Overwriting.");
            }

            _pageRegistry[key] = pageType;
            Debug.WriteLine($"NavigationService: Registered page '{key}' -> {pageType.Name}.");
        }

        public void UnregisterPage(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Page key cannot be null or empty.", nameof(key));
            }

            if (_pageRegistry.Remove(key))
            {
                Debug.WriteLine($"NavigationService: Unregistered page '{key}'.");
            }
            else
            {
                Debug.WriteLine($"NavigationService: Page key '{key}' was not registered.");
            }
        }

        private void OnFrameNavigated(object sender, NavigationEventArgs e)
        {
            if (e.SourcePageType != null)
            {
                Navigated?.Invoke(this, e.SourcePageType);
                Debug.WriteLine($"NavigationService: Navigated event raised for {e.SourcePageType.Name}.");
            }
        }
    }
}
