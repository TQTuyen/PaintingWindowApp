using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using PaintingApp.Contracts;

namespace PaintingApp.ViewModels;

public abstract partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _title = string.Empty;

    protected INavigationService NavigationService { get; }
    protected IDialogService DialogService { get; }

    protected BaseViewModel(INavigationService navigationService, IDialogService dialogService)
    {
        NavigationService = navigationService;
        DialogService = dialogService;
    }

    public virtual void OnNavigatedTo(object? parameter)
    {
    }

    public virtual void OnNavigatedFrom()
    {
    }

    public virtual Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Executes an async operation with automatic busy state management and error handling.
    /// </summary>
    protected async Task ExecuteAsync(Func<Task> operation, Action<Exception>? onError = null)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            await operation();
        }
        catch (Exception ex)
        {
            if (onError != null)
            {
                onError(ex);
            }
            else
            {
                await DialogService.ShowErrorAsync("Error", ex.Message);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Executes an async operation that returns a value with automatic busy state management and error handling.
    /// </summary>
    protected async Task<T?> ExecuteAsync<T>(Func<Task<T>> operation, Action<Exception>? onError = null)
    {
        if (IsBusy)
            return default;

        try
        {
            IsBusy = true;
            return await operation();
        }
        catch (Exception ex)
        {
            if (onError != null)
            {
                onError(ex);
            }
            else
            {
                await DialogService.ShowErrorAsync("Error", ex.Message);
            }
            return default;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
